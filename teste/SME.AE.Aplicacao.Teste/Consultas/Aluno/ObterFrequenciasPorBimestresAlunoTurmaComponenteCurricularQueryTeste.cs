using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryTeste
    {
        [Fact]
        public void Construtor_FactoryNula_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_RespostaSucesso_DeveDesserializarEEnviarFiltros()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.OK, "[{\"CodigoAluno\":\"A1\",\"TotalAulas\":10,\"TotalAusencias\":1}]");
            var handler = new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery([1, 4], "T1", "A1", "C1"), CancellationToken.None);

            var frequencia = Assert.Single(resultado);
            Assert.Equal("A1", frequencia.CodigoAluno);
            Assert.Equal(10, frequencia.TotalAulas);
            Assert.Contains("turmas/T1/alunos/A1/componentes-curriculares/C1", factory.Handler.UltimaRequisicao.RequestUri.ToString());
            Assert.Contains("bimestres=1&bimestres=4", factory.Handler.UltimaRequisicao.RequestUri.ToString());
        }

        [Fact]
        public async Task Handle_RespostaNaoSucesso_DeveLancarExcecao()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.BadRequest, "");
            var handler = new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler(factory.Factory.Object);

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery([1], "T1", "A1", "C1"), CancellationToken.None));

            Assert.Equal("Não foi possível localizar as frequências do aluno : A1 da turma T1.", excecao.Message);
        }
    }
}