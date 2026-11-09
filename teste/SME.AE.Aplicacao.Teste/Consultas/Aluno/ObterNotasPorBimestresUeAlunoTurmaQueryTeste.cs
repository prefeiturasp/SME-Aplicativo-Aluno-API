using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    public class ObterNotasPorBimestresUeAlunoTurmaQueryTeste
    {
        [Fact]
        public void Construtor_FactoryNula_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterNotasPorBimestresUeAlunoTurmaQueryHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_RespostaSucesso_DeveDesserializarEEnviarBimestres()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.OK, "[{\"ComponenteCurricularCodigo\":10,\"ComponenteCurricularNome\":\"Matemática\",\"Nota\":\"8\"}]");
            var handler = new ObterNotasPorBimestresUeAlunoTurmaQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterNotasPorBimestresUeAlunoTurmaQuery(new[] { 1, 2 }, "T1", "U1", "A1"), CancellationToken.None);

            var nota = Assert.Single(resultado);
            Assert.Equal(10, nota.ComponenteCurricularCodigo);
            Assert.Equal("8", nota.Nota);
            Assert.Contains("ues/U1/turmas/T1/alunos/A1", factory.Handler.UltimaRequisicao.RequestUri.ToString());
            Assert.Contains("bimestres=1&bimestres=2", factory.Handler.UltimaRequisicao.RequestUri.ToString());
        }

        [Fact]
        public async Task Handle_RespostaNaoSucesso_DeveLancarExcecao()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.BadRequest, "");
            var handler = new ObterNotasPorBimestresUeAlunoTurmaQueryHandler(factory.Factory.Object);

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new ObterNotasPorBimestresUeAlunoTurmaQuery(new[] { 1 }, "T1", "U1", "A1"), CancellationToken.None));

            Assert.Equal("Não foi possível localizar as notas do aluno : A1 da turma T1.", excecao.Message);
        }
    }
}