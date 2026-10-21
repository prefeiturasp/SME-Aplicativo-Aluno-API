using MediatR;
using Moq;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Usuario
{
    public class AtualizarDadosUsuarioEolUseCaseTeste : BaseTeste
    {
        private const int IdUsuario = 1;
        private const string CpfUsuario = "12345678900";
        private const string CpfResponsavelEol = "98765432100";
        private const string CodigoAluno = "1234567";
        private const string OutroCodigoAluno = "7654321";
        private static readonly string MensagemJsonNula = JsonConvert.SerializeObject(null);

        private readonly IAtualizarDadosUsuarioEolUseCase atualizarDadosUsuarioEolUseCase;
        private readonly List<AtualizarDadosResponsavelEolCommand> atualizacoesEnviadas;

        public AtualizarDadosUsuarioEolUseCaseTeste()
        {
            atualizarDadosUsuarioEolUseCase = new AtualizarDadosUsuarioEolUseCase(mediator.Object);
            atualizacoesEnviadas = new List<AtualizarDadosResponsavelEolCommand>();

            mediator.Setup(m => m.Send(It.IsAny<AtualizarDadosResponsavelEolCommand>(), It.IsAny<CancellationToken>()))
                    .Callback<IRequest<bool>, CancellationToken>((request, token) => atualizacoesEnviadas.Add((AtualizarDadosResponsavelEolCommand)request))
                    .ReturnsAsync(true);
        }

        [Fact(DisplayName = "Deve buscar o usuário pelo Id da mensagem e os responsáveis pelo CPF do usuário")]
        public async Task Deve_Buscar_O_Usuario_Pelo_Id_Da_Mensagem_E_Os_Responsaveis_Pelo_CPF_Do_Usuario()
        {
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(CriarResponsavel(CodigoAluno));

            await atualizarDadosUsuarioEolUseCase.Executar(CriarMensagem(CriarDto()));

            mediator.Verify(m => m.Send(It.Is<ObterUsuarioQuery>(q => q.Id == IdUsuario && q.Cpf == null), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.Is<ObterDadosResponsavelQuery>(q => q.CpfResponsavel == CpfUsuario), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve enviar uma atualização ao EOL para cada aluno vinculado ao responsável")]
        public async Task Deve_Enviar_Uma_Atualizacao_Ao_EOL_Para_Cada_Aluno_Vinculado_Ao_Responsavel()
        {
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(CriarResponsavel(CodigoAluno), CriarResponsavel(OutroCodigoAluno));

            var resultado = await atualizarDadosUsuarioEolUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.True(resultado);
            Assert.Equal(new[] { CodigoAluno, OutroCodigoAluno }, atualizacoesEnviadas.Select(a => a.CodigoAluno));
        }

        [Fact(DisplayName = "Deve aplicar no EOL os dados informados pelo responsável")]
        public async Task Deve_Aplicar_No_EOL_Os_Dados_Informados_Pelo_Responsavel()
        {
            var dadosAtualizacao = CriarDto();
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(CriarResponsavel(CodigoAluno));

            await atualizarDadosUsuarioEolUseCase.Executar(CriarMensagem(dadosAtualizacao));

            var atualizacao = Assert.Single(atualizacoesEnviadas);
            Assert.Equal(CodigoAluno, atualizacao.CodigoAluno);
            Assert.Equal(long.Parse(CpfResponsavelEol), atualizacao.Cpf);
            Assert.Equal(dadosAtualizacao.Email, atualizacao.Email);
            Assert.Equal(dadosAtualizacao.NomeMae, atualizacao.NomeMae);
            Assert.Equal(dadosAtualizacao.DataNascimentoResponsavel, atualizacao.DataNascimentoResponsavel);
            Assert.Equal("11", atualizacao.DDD);
            Assert.Equal("987654321", atualizacao.Celular);
        }

        [Fact(DisplayName = "Deve retornar false sem consultar dados quando a mensagem não tiver conteúdo")]
        public async Task Deve_Retornar_False_Sem_Consultar_Dados_Quando_A_Mensagem_Nao_Tiver_Conteudo()
        {
            var resultado = await atualizarDadosUsuarioEolUseCase.Executar(new MensagemRabbit(MensagemJsonNula));

            Assert.False(resultado);
            mediator.Verify(m => m.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Empty(atualizacoesEnviadas);
        }

        [Fact(DisplayName = "Deve retornar false quando o usuário não existir na aplicação")]
        public async Task Deve_Retornar_False_Quando_O_Usuario_Nao_Existir_Na_Aplicacao()
        {
            MockarUsuario(null);

            var resultado = await atualizarDadosUsuarioEolUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.False(resultado);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.Empty(atualizacoesEnviadas);
        }

        [Theory(DisplayName = "Deve retornar false quando não houver responsáveis no EOL")]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Deve_Retornar_False_Quando_Nao_Houver_Responsaveis_No_EOL(bool retornoNulo)
        {
            MockarUsuario(CriarUsuario());
            MockarResponsaveisEol(retornoNulo ? null : Array.Empty<DadosResponsavelAluno>());

            var resultado = await atualizarDadosUsuarioEolUseCase.Executar(CriarMensagem(CriarDto()));

            Assert.False(resultado);
            Assert.Empty(atualizacoesEnviadas);
        }

        private void MockarUsuario(Dominio.Entidades.Usuario usuario)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(usuario);
        }

        private void MockarResponsaveisEol(params DadosResponsavelAluno[] responsaveis)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(responsaveis);
        }

        private static MensagemRabbit CriarMensagem(AtualizarDadosUsuarioDto dadosAtualizacao)
        {
            return new MensagemRabbit(JsonConvert.SerializeObject(dadosAtualizacao), Guid.NewGuid());
        }

        private static AtualizarDadosUsuarioDto CriarDto()
        {
            return new AtualizarDadosUsuarioDto
            {
                Id = IdUsuario,
                Email = "responsavel@teste.com",
                NomeMae = "Maria da Silva",
                Celular = "(11) 98765-4321",
                DataNascimentoResponsavel = new DateTime(1985, 5, 10)
            };
        }

        private static Dominio.Entidades.Usuario CriarUsuario()
        {
            return new Dominio.Entidades.Usuario
            {
                Id = IdUsuario,
                Cpf = CpfUsuario
            };
        }

        private static DadosResponsavelAluno CriarResponsavel(string codigoAluno)
        {
            return new DadosResponsavelAluno
            {
                CodigoAluno = codigoAluno,
                Cpf = CpfResponsavelEol,
                Nome = "Responsável EOL",
                Email = "email.antigo@teste.com",
                NomeMae = "Nome da Mãe Antigo",
                DDDCelular = "21",
                NumeroCelular = "912345678"
            };
        }
    }
}
