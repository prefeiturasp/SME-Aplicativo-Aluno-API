using MediatR;
using Moq;
using SME.AE.Aplicacao.Comum;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_QuandoConsultaRetornarNulo_DeveRetornarNuloSemComplementos()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<FrequenciaAlunoDto>)null);
            var useCase = new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FrequenciaPorBimestresAlunoTurmaComponenteCurricularDto("T1", "A1", [1], "C1"));

            Assert.Null(resultado);
            mediator.Verify(x => x.Send(It.IsAny<ObterTurmasModalidadesPorCodigosQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Executar_ComFrequencias_DeveAtualizarCorDeCadaRegistro()
        {
            var frequencias = new List<FrequenciaAlunoDto>
            {
                new() { CodigoAluno = "A1", TotalAulas = 10, TotalAusencias = 1 },
                new() { CodigoAluno = "A1", TotalAulas = 10, TotalAusencias = 2 }
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(frequencias);
            mediator.Setup(x => x.Send(It.IsAny<ObterTurmasModalidadesPorCodigosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([ new TurmaModalidadeCodigoDto { ModalidadeCodigo = (int)ModalidadeDeEnsino.Fundamental } ]);
            mediator.Setup(x => x.Send(It.IsAny<ObterParametrosSistemaPorChavesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<ParametroEscolaAqui>());
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciaAlunoCorPorParametroQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<FrequenciaAlunoCor>());
            mediator.Setup(x => x.Send(It.IsAny<ObterFrequenciaAlunoFaixaPorParametroQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<FrequenciaAlunoFaixa>());
            mediator.Setup(x => x.Send(It.IsAny<ObterCorQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("#FF0000");
            var useCase = new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase(mediator.Object);

            var resultado = await useCase.Executar(new FrequenciaPorBimestresAlunoTurmaComponenteCurricularDto("T1", "A1", [1], "C1"));

            Assert.Equal(2, Assert.IsType<IEnumerable<FrequenciaAlunoDto>>(resultado, exactMatch: false).Count());
            Assert.All(resultado, frequencia => Assert.Equal("#FF0000", frequencia.CorDaFrequencia));
            mediator.Verify(x => x.Send(It.Is<ObterCorQuery>(q => q.Frequencia == 90 || q.Frequencia == 80), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}