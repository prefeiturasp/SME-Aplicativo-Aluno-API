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
    public class ObterTermosDeUsoUseCaseTeste
    {
        private readonly Mock<IMediator> mediator;
        private readonly ObterTermosDeUsoUseCase useCase;

        public ObterTermosDeUsoUseCaseTeste()
        {
            mediator = new Mock<IMediator>();
            useCase = new ObterTermosDeUsoUseCase(mediator.Object);
        }

        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterTermosDeUsoUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_DeveRetornarTermosDeUso()
        {
            var retorno = new RetornoTermosDeUsoDto("termos", "politica", 1.0, 1);
            mediator.Setup(x => x.Send(It.IsAny<ObterTermosDeUsoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(retorno);

            var resultado = await useCase.Executar();

            Assert.Same(retorno, resultado);
            mediator.Verify(x => x.Send(It.IsAny<ObterTermosDeUsoQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoMediatorFalhar_DevePropagarExcecao()
        {
            mediator.Setup(x => x.Send(It.IsAny<ObterTermosDeUsoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("falha ao consultar termos"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Executar());

            Assert.Equal("falha ao consultar termos", excecao.Message);
        }
    }
}
