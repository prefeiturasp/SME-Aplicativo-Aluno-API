using MediatR;
using Moq;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class ObterNotasPorBimestresUeAlunoTurmaUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterNotasPorBimestresUeAlunoTurmaUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_DeveDefinirCoresParaNotasNumericasEConceitos()
        {
            var notas = new[]
            {
                new NotaConceitoBimestreComponenteDto { Nota = "4,99" },
                new NotaConceitoBimestreComponenteDto { Nota = "5,00" },
                new NotaConceitoBimestreComponenteDto { Nota = "7,00" },
                new NotaConceitoBimestreComponenteDto { ConceitoId = 1 },
                new NotaConceitoBimestreComponenteDto { Nota = "desconhecido" }
            };
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterNotasPorBimestresUeAlunoTurmaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(notas);
            mediator.Setup(x => x.Send(It.IsAny<ObterNotaAlunoCoresQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                [
                    new NotaAlunoCor { Nota = NotaAlunoCor.NotaAbaixo5, Cor = "#111111" },
                    new NotaAlunoCor { Nota = NotaAlunoCor.NotaEntre7e5, Cor = "#222222" },
                    new NotaAlunoCor { Nota = NotaAlunoCor.NotaAcimaDe7, Cor = "#333333" },
                    new NotaAlunoCor { Nota = "P", Cor = "#444444" }
                ]);
            var useCase = new ObterNotasPorBimestresUeAlunoTurmaUseCase(mediator.Object);

            var resultado = (await useCase.Executar(new AlunoBimestresTurmaDto("U1", "T1", "A1", [1, 2]))).ToList();

            Assert.Equal("#111111", resultado[0].CorDaNota);
            Assert.Equal("#222222", resultado[1].CorDaNota);
            Assert.Equal("#333333", resultado[2].CorDaNota);
            Assert.Equal("#444444", resultado[3].CorDaNota);
            Assert.Equal(NotaAlunoCor.CorPadrao, resultado[4].CorDaNota);
            mediator.Verify(x => x.Send(It.Is<ObterNotasPorBimestresUeAlunoTurmaQuery>(q => q.UeCodigo == "U1" && q.TurmaCodigo == "T1" && q.AlunoCodigo == "A1" && q.Bimestres.SequenceEqual(new[] { 1, 2 })), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<ObterNotaAlunoCoresQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoNaoHouverNotas_DeveRetornarListaVazia()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterNotasPorBimestresUeAlunoTurmaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<NotaConceitoBimestreComponenteDto>());
            mediator.Setup(x => x.Send(It.IsAny<ObterNotaAlunoCoresQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<NotaAlunoCor>());
            var useCase = new ObterNotasPorBimestresUeAlunoTurmaUseCase(mediator.Object);

            var resultado = await useCase.Executar(new AlunoBimestresTurmaDto("U1", "T1", "A1", [1]));

            Assert.Empty(resultado);
        }
    }
}