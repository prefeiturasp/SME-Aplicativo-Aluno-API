using Bogus;
using Moq;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using MediatR;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Comum;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Usuario
{
    public class AtualizarDadosUsuarioUseCaseTeste : BaseTeste
    {
        private const string CpfUsuario = "12345678900";
        private const string EtapaSalvarUsuario = "SalvarUsuario";
        private const string MensagemUsuarioNaoEncontradoNaAplicacao = "Usuário não encontrado na base de dados da aplicação!";
        private const string MensagemResponsavelNaoEncontrado = "Usuário não encontrado!";

        private readonly AtualizarDadosUsuarioUseCase atualizarDadosUsuarioUseCase;

        public AtualizarDadosUsuarioUseCaseTeste()
        {
            atualizarDadosUsuarioUseCase = new AtualizarDadosUsuarioUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Deve Atualizar Dados do Usuário")]
        public async Task Deve_Atualizar_Dados_Do_Usuario()
        {
            var dadosAtualizacao = GerarAtualizarDadosUsuarioDto();
            var usuario = CriarUsuario();
            MockarUsuario(usuario);
            MockarResponsavelEol(DadosResponsavelResumidoMock(CpfUsuario));

            var resultado = await atualizarDadosUsuarioUseCase.Executar(dadosAtualizacao);

            Assert.True(resultado.Ok);
            var respostaAutenticar = Assert.IsType<RespostaAutenticar>(resultado.Data);
            Assert.Equal(usuario.Cpf, respostaAutenticar.Cpf);
            Assert.Equal(usuario.Id, respostaAutenticar.Id);

            mediator.Verify(m => m.Send(It.Is<ObterUsuarioQuery>(q => q.Id == dadosAtualizacao.Id && q.Cpf == null), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.Is<ObterDadosResponsavelResumidoQuery>(q => q.CpfResponsavel == CpfUsuario), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve atualizar somente a auditoria do usuário antes de salvar")]
        public async Task Deve_Atualizar_Somente_A_Auditoria_Do_Usuario_Antes_De_Salvar()
        {
            var usuario = CriarUsuario();
            var criadoEm = usuario.CriadoEm;
            var ultimoLogin = usuario.UltimoLogin;
            var validadeToken = usuario.ValidadeToken;
            MockarUsuario(usuario);
            MockarResponsavelEol(DadosResponsavelResumidoMock(CpfUsuario));

            Dominio.Entidades.Usuario usuarioSalvo = null;
            DateTime? alteradoEmNoSalvar = null;
            string alteradoPorNoSalvar = null;
            mediator.Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<Unit>, CancellationToken>((request, token) =>
                    {
                        usuarioSalvo = ((SalvarUsuarioCommand)request).Usuario;
                        alteradoEmNoSalvar = usuarioSalvo.AlteradoEm;
                        alteradoPorNoSalvar = usuarioSalvo.AlteradoPor;
                    })
                    .ReturnsAsync(Unit.Value);

            var inicioExecucao = DateTime.Now;
            var resultado = await atualizarDadosUsuarioUseCase.Executar(GerarAtualizarDadosUsuarioDto());
            var fimExecucao = DateTime.Now;

            Assert.Same(usuario, usuarioSalvo);
            Assert.Equal("Sistema", alteradoPorNoSalvar);
            Assert.NotNull(alteradoEmNoSalvar);
            Assert.InRange(alteradoEmNoSalvar.Value, inicioExecucao, fimExecucao);
            Assert.Equal(CpfUsuario, usuario.Cpf);
            Assert.Equal(criadoEm, usuario.CriadoEm);
            Assert.Equal(ultimoLogin, usuario.UltimoLogin);
            Assert.Equal("tokenAntigo", usuario.Token);
            Assert.Equal(validadeToken, usuario.ValidadeToken);
            Assert.False(usuario.Excluido);
            Assert.False(usuario.RedefinirSenha);

            var respostaAutenticar = Assert.IsType<RespostaAutenticar>(resultado.Data);
            Assert.Equal(alteradoEmNoSalvar, respostaAutenticar.UltimaAtualizacao);
            Assert.Empty(resultado.Erros);
        }

        [Fact(DisplayName = "Deve publicar nas filas do EOL e da Prodam o mesmo DTO informado com o mesmo código de correlação")]
        public async Task Deve_Publicar_Nas_Filas_Do_EOL_E_Da_Prodam_O_Mesmo_DTO_Com_O_Mesmo_Codigo_De_Correlacao()
        {
            MockarUsuario(CriarUsuario());
            MockarResponsavelEol(DadosResponsavelResumidoMock(CpfUsuario));

            var publicacoes = new List<PublicarFilaAeCommand>();
            mediator.Setup(m => m.Send(It.IsAny<PublicarFilaAeCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<bool>, CancellationToken>((request, token) => publicacoes.Add((PublicarFilaAeCommand)request))
                    .ReturnsAsync(true);

            var dadosAtualizacao = GerarAtualizarDadosUsuarioDto();

            await atualizarDadosUsuarioUseCase.Executar(dadosAtualizacao);

            Assert.Equal(2, publicacoes.Count);
            var publicacaoEol = Assert.Single(publicacoes, p => p.Rota == RotasRabbitAe.RotaAtualizacaoCadastralEol);
            var publicacaoProdam = Assert.Single(publicacoes, p => p.Rota == RotasRabbitAe.RotaAtualizacaoCadastralProdam);
            Assert.Same(dadosAtualizacao, publicacaoEol.Mensagem);
            Assert.Same(dadosAtualizacao, publicacaoProdam.Mensagem);
            Assert.NotEqual(Guid.Empty, publicacaoEol.CodigoCorrelacao);
            Assert.Equal(publicacaoEol.CodigoCorrelacao, publicacaoProdam.CodigoCorrelacao);
        }

        [Fact(DisplayName = "Deve salvar o usuário antes de publicar nas filas do EOL e da Prodam")]
        public async Task Deve_Salvar_O_Usuario_Antes_De_Publicar_Nas_Filas_Do_EOL_E_Da_Prodam()
        {
            MockarUsuario(CriarUsuario());
            MockarResponsavelEol(DadosResponsavelResumidoMock(CpfUsuario));

            var etapas = new List<string>();
            mediator.Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                    .Callback(() => etapas.Add(EtapaSalvarUsuario))
                    .ReturnsAsync(Unit.Value);
            mediator.Setup(m => m.Send(It.IsAny<PublicarFilaAeCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<bool>, CancellationToken>((request, token) => etapas.Add(((PublicarFilaAeCommand)request).Rota))
                    .ReturnsAsync(true);

            await atualizarDadosUsuarioUseCase.Executar(GerarAtualizarDadosUsuarioDto());

            Assert.Equal(new[] { EtapaSalvarUsuario, RotasRabbitAe.RotaAtualizacaoCadastralEol, RotasRabbitAe.RotaAtualizacaoCadastralProdam }, etapas);
        }

        [Fact(DisplayName = "Deve retornar falha quando o usuário não existir na aplicação")]
        public async Task Deve_Retornar_Falha_Quando_O_Usuario_Nao_Existir_Na_Aplicacao()
        {
            MockarUsuario(null);

            var resultado = await atualizarDadosUsuarioUseCase.Executar(GerarAtualizarDadosUsuarioDto());

            Assert.False(resultado.Ok);
            Assert.Equal(new[] { MensagemUsuarioNaoEncontradoNaAplicacao }, resultado.Erros);
            Assert.Null(resultado.Data);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosResponsavelResumidoQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            ValidarQueNaoSalvouNemPublicou();
        }

        [Fact(DisplayName = "Deve retornar falha quando o responsável não for encontrado no EOL")]
        public async Task Deve_Retornar_Falha_Quando_O_Responsavel_Nao_For_Encontrado_No_EOL()
        {
            MockarUsuario(CriarUsuario());
            MockarResponsavelEol(null);

            var resultado = await atualizarDadosUsuarioUseCase.Executar(GerarAtualizarDadosUsuarioDto());

            Assert.False(resultado.Ok);
            Assert.Equal(new[] { MensagemResponsavelNaoEncontrado }, resultado.Erros);
            Assert.Null(resultado.Data);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosResponsavelResumidoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            ValidarQueNaoSalvouNemPublicou();
        }

        private void MockarUsuario(Dominio.Entidades.Usuario usuario)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);
        }

        private void MockarResponsavelEol(DadosResponsavelAlunoResumido responsavel)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterDadosResponsavelResumidoQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(responsavel);
        }

        private void ValidarQueNaoSalvouNemPublicou()
        {
            mediator.Verify(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            mediator.Verify(m => m.Send(It.IsAny<PublicarFilaAeCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private static Dominio.Entidades.Usuario CriarUsuario()
        {
            return new Dominio.Entidades.Usuario
            {
                Id = 1,
                Cpf = CpfUsuario,
                UltimoLogin = DateTime.Now.AddDays(-5),
                CriadoEm = DateTime.Now.AddMonths(-1),
                AlteradoEm = DateTime.Now.AddDays(-5),
                Excluido = false,
                RedefinirSenha = false,
                Token = "tokenAntigo",
                ValidadeToken = DateTime.Now.AddDays(1)
            };
        }

        private AtualizarDadosUsuarioDto GerarAtualizarDadosUsuarioDto()
        {
            var faker = new Faker<AtualizarDadosUsuarioDto>("pt_BR")
                .RuleFor(u => u.Id, f => 1)
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.DataNascimentoResponsavel, f => f.Date.Past(30, DateTime.Now.AddYears(-18)))
                .RuleFor(u => u.NomeMae, f => f.Name.FullName())
                .RuleFor(u => u.Celular, f => f.Phone.PhoneNumber("9########"));

            return faker.Generate();
        }

        private DadosResponsavelAlunoResumido DadosResponsavelResumidoMock(string cpf)
        {
            return new DadosResponsavelAlunoResumido
            {
                NumeroCelular = "999999999",
                DDDCelular = "99",
                Email = "email.eol@teste.com",
                Nome = "Nome do Responsável EOL",
                Cpf = cpf
            };
        }
    }
}
