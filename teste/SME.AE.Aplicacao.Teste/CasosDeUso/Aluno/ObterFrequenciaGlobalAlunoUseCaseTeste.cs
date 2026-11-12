using MediatR;
using Moq;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class ObterFrequenciaGlobalAlunoUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterFrequenciaGlobalAlunoUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_QuandoNaoHouverFrequencia_DeveRetornarNuloSemConsultarComplementos()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciaGlobalAlunoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((FrequenciaGlobalDto)null);
            var useCase = new ObterFrequenciaGlobalAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FiltroFrequenciaGlobalAlunoDto { TurmaCodigo = "T1", AlunoCodigo = "A1" });

            Assert.Null(resultado);
            mediator.Verify(x => x.Send(It.IsAny<ObterTurmasModalidadesPorCodigosQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Executar_EnsinoInfantil_DeveUsarParametrosInfantisEAplicarCor()
        {
            var mediator = CriarMediatorComFrequencia(ModalidadeDeEnsino.Infantil);
            var useCase = new ObterFrequenciaGlobalAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FiltroFrequenciaGlobalAlunoDto { TurmaCodigo = "T1", AlunoCodigo = "A1" });

            Assert.Equal("#00FF00", resultado.CorDaFrequencia);
            mediator.Verify(x => x.Send(It.Is<ObterParametrosSistemaPorChavesQuery>(q => q.Chaves.SequenceEqual(FrequenciaAlunoCor.ObterChavesDosParametrosParaEnsinoInfantil())), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_EnsinoNaoInfantil_DeveUsarParametrosPadraoEAplicarCor()
        {
            var mediator = CriarMediatorComFrequencia(ModalidadeDeEnsino.Fundamental);
            var useCase = new ObterFrequenciaGlobalAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FiltroFrequenciaGlobalAlunoDto { TurmaCodigo = "T1", AlunoCodigo = "A1" });

            Assert.Equal("#00FF00", resultado.CorDaFrequencia);
            mediator.Verify(x => x.Send(It.Is<ObterParametrosSistemaPorChavesQuery>(q => q.Chaves.SequenceEqual(FrequenciaAlunoCor.ObterChavesDosParametros())), It.IsAny<CancellationToken>()), Times.Once);
        }

        private static Mock<IMediator> CriarMediatorComFrequencia(ModalidadeDeEnsino modalidade)
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciaGlobalAlunoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new FrequenciaGlobalDto { Frequencia = 95 });
            mediator.Setup(x => x.Send(It.IsAny<ObterTurmasModalidadesPorCodigosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([ new TurmaModalidadeCodigoDto { TurmaCodigo = 1, ModalidadeCodigo = (int)modalidade } ]);
            mediator.Setup(x => x.Send(It.IsAny<ObterParametrosSistemaPorChavesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ParametroEscolaAqui>());
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciaAlunoCorPorParametroQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<FrequenciaAlunoCor>());
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciaAlunoFaixaPorParametroQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<FrequenciaAlunoFaixa>());
            mediator.Setup(x => x.Send(It.IsAny<ObterCorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("#00FF00");
            return mediator;
        }
    }
}