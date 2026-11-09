using MediatR;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.TermosDeUso;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.TermoDeUso
{
    public class ObterTermosDeUsoPorCpfUseCaseTeste
    {
        private readonly Mock<IMediator> mediator;
        private readonly ObterTermosDeUsoPorCpfUseCase useCase;

        public ObterTermosDeUsoPorCpfUseCaseTeste()
        {
            mediator = new Mock<IMediator>();
            useCase = new ObterTermosDeUsoPorCpfUseCase(mediator.Object);
        }

        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterTermosDeUsoPorCpfUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_CpfValido_DeveRetornarTermosDeUso()
        {
            var retorno = new RetornoTermosDeUsoDto("termos", "politica", 1.0, 1);
            mediator.Setup(x => x.Send(It.IsAny<ObterTermosDeUsoPorCpfQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);

            var resultado = await useCase.Executar("12345678901");

            Assert.Same(retorno, resultado);
            mediator.Verify(x => x.Send(
                It.Is<ObterTermosDeUsoPorCpfQuery>(query => query.CPF == "12345678901"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoMediatorFalhar_DevePropagarExcecao()
        {
            mediator.Setup(x => x.Send(It.IsAny<ObterTermosDeUsoPorCpfQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("falha ao consultar termos"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Executar("12345678901"));

            Assert.Equal("falha ao consultar termos", excecao.Message);
        }
    }
}
