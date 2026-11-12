using MediatR;
using Moq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class ObterComponentesCurricularesIdsUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterComponentesCurricularesIdsUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_DeveMapearEEliminarComponentesDuplicados()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterNotasPorBimestresUeAlunoTurmaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([

                    new NotaConceitoBimestreComponenteDto { ComponenteCurricularCodigo = 10, ComponenteCurricularNome = "Matemática" },
                    new NotaConceitoBimestreComponenteDto { ComponenteCurricularCodigo = 10, ComponenteCurricularNome = "Matemática" },
                    new NotaConceitoBimestreComponenteDto { ComponenteCurricularCodigo = 20, ComponenteCurricularNome = "História" }
                ]);
            var useCase = new ObterComponentesCurricularesIdsUseCase(mediator.Object);

            var resultado = (await useCase.Executar(new AlunoBimestresTurmaDto("U1", "T1", "A1", [1, 2]))).ToList();

            Assert.Equal(2, resultado.Count);
            Assert.Contains(resultado, x => x.Codigo == 10 && x.Descricao == "Matemática");
            Assert.Contains(resultado, x => x.Codigo == 20 && x.Descricao == "História");
            mediator.Verify(x => x.Send(It.Is<ObterNotasPorBimestresUeAlunoTurmaQuery>(q => q.UeCodigo == "U1" && q.TurmaCodigo == "T1" && q.AlunoCodigo == "A1" && q.Bimestres.Length == 2 && q.Bimestres[0] == 1 && q.Bimestres[1] == 2), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoNaoHouverNotas_DeveRetornarListaVazia()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<ObterNotasPorBimestresUeAlunoTurmaQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<NotaConceitoBimestreComponenteDto>());
            var useCase = new ObterComponentesCurricularesIdsUseCase(mediator.Object);

            var resultado = await useCase.Executar(new AlunoBimestresTurmaDto("U1", "T1", "A1", [1]));

            Assert.Empty(resultado);
        }
    }
}