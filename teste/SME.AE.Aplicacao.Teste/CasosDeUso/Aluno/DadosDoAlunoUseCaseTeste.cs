using MediatR;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class DadosDoAlunoUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new DadosDoAlunoUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_DeveEnviarCpfERetornarResposta()
        {
            var mediator = new Mock<IMediator>();
            var resposta = RespostaApi.Sucesso(new object());
            mediator.Setup(x => x.Send(It.Is<DadosAlunoCommand>(c => c.Cpf == "12345678901"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(resposta);
            var useCase = new DadosDoAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar("12345678901");

            Assert.Same(resposta, resultado);
            mediator.Verify(x => x.Send(It.Is<DadosAlunoCommand>(c => c.Cpf == "12345678901"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}