using Bogus;
using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
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
        private readonly AtualizarDadosUsuarioUseCase atualizarDadosUsuarioUseCase;

        public AtualizarDadosUsuarioUseCaseTeste()
        {
            atualizarDadosUsuarioUseCase = new AtualizarDadosUsuarioUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Deve Atualizar Dados do Usuário")]
        public async Task Deve_Atualizar_Dados_Do_Usuario()
        {
            var dadosAtualizacao = GerarAtualizarDadosUsuarioDto();

            var usuarioExistente = new Dominio.Entidades.Usuario
            {
                Id = dadosAtualizacao.Id,
                Cpf = "12345678900",
                UltimoLogin = DateTime.Now.AddDays(-5),
                CriadoEm = DateTime.Now.AddMonths(-1),
                AlteradoEm = DateTime.Now.AddDays(-5),
                Excluido = false,
                RedefinirSenha = false,
                Token = "tokenAntigo",
                ValidadeToken = DateTime.Now.AddDays(1)
            };

            mediator.Setup(m => m.Send(It.Is<ObterUsuarioQuery>(q => q.Id == dadosAtualizacao.Id), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuarioExistente);

            var dadosResponsavelResumido = DadosResponsavelResumidoMock(usuarioExistente.Cpf);
            mediator.Setup(m => m.Send(It.Is<ObterDadosResponsavelResumidoQuery>(q => q.CpfResponsavel == usuarioExistente.Cpf), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(dadosResponsavelResumido);

            Dominio.Entidades.Usuario usuarioCapturado = null;
            mediator.Setup(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<Unit>, CancellationToken>((request, token) =>
                    {
                        var command = request as SalvarUsuarioCommand;
                        if (command != null)
                        {
                            usuarioCapturado = command.Usuario;
                        }
                    })
                    .ReturnsAsync(Unit.Value);

            var resultado = await atualizarDadosUsuarioUseCase.Executar(dadosAtualizacao);

            Assert.NotNull(resultado);
            Assert.True(resultado.Ok);
            Assert.NotNull(resultado.Data);

            mediator.Verify(m => m.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosResponsavelResumidoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.IsAny<SalvarUsuarioCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.Is<PublicarFilaAeCommand>(c => c.Rota == RotasRabbitAe.RotaAtualizacaoCadastralEol), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.Is<PublicarFilaAeCommand>(c => c.Rota == RotasRabbitAe.RotaAtualizacaoCadastralProdam), It.IsAny<CancellationToken>()), Times.Once);

            Assert.NotNull(usuarioCapturado);
            Assert.Equal(usuarioExistente.Cpf, usuarioCapturado.Cpf);
            Assert.Equal(usuarioExistente.Id, usuarioCapturado.Id);


            var respostaAutenticar = resultado.Data as RespostaAutenticar;
            Assert.NotNull(respostaAutenticar);
            Assert.Equal(usuarioCapturado.Cpf, respostaAutenticar.Cpf);
            Assert.Equal(usuarioCapturado.Id, respostaAutenticar.Id);
            Assert.Equal(usuarioCapturado.AlteradoEm, respostaAutenticar.UltimaAtualizacao);
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
