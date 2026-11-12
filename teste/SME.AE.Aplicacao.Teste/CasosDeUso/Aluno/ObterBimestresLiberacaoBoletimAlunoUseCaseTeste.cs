using MediatR;
using Moq;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Aluno
{
    public class ObterBimestresLiberacaoBoletimAlunoUseCaseTeste
    {
        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterBimestresLiberacaoBoletimAlunoUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_DeveEnviarTurmaERetornarBimestres()
        {
            var mediator = new Mock<IMediator>();
            int[] esperado = [1, 3];
            mediator.Setup(x => x.Send(It.Is<ObterBimestresLiberacaoBoletimQuery>(q => q.TurmaCodigo == "T1"), It.IsAny<CancellationToken>()))
                .ReturnsAsync(esperado);
            var useCase = new ObterBimestresLiberacaoBoletimAlunoUseCase(mediator.Object);

            var resultado = await useCase.Executar("T1");

            Assert.Equal(esperado, resultado);
            mediator.Verify(x => x.Send(It.Is<ObterBimestresLiberacaoBoletimQuery>(q => q.TurmaCodigo == "T1"), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}