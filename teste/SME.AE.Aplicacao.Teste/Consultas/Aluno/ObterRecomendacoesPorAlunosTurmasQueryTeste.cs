using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    public class ObterRecomendacoesPorAlunosTurmasQueryTeste
    {
        [Fact]
        public void Construtor_FactoryNula_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterRecomendacoesPorAlunosTurmasQueryHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_RespostaSucesso_DeveDesserializarEEnviarFiltros()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.OK, "[{\"AlunoCodigo\":\"A1\",\"TurmaCodigo\":\"T1\"}]");
            var handler = new ObterRecomendacoesPorAlunosTurmasQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterRecomendacoesPorAlunosTurmasQuery("A1", "T1", 2025, ModalidadeDeEnsino.Fundamental, 2), CancellationToken.None);

            var recomendacao = Assert.Single(resultado);
            Assert.Equal("A1", recomendacao.AlunoCodigo);
            var url = factory.Handler.UltimaRequisicao.RequestUri.ToString();
            Assert.Contains("codigoAluno=A1", url);
            Assert.Contains("codigoTurma=T1", url);
            Assert.Contains("anoLetivo=2025", url);
            Assert.Contains("modalidade=5", url);
            Assert.Contains("semestre=2", url);
        }

        [Fact]
        public async Task Handle_RespostaNaoSucesso_DeveLancarExcecao()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.BadRequest, "");
            var handler = new ObterRecomendacoesPorAlunosTurmasQueryHandler(factory.Factory.Object);

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new ObterRecomendacoesPorAlunosTurmasQuery("A1", "T1", 2025, ModalidadeDeEnsino.Fundamental, 1), CancellationToken.None));

            Assert.Equal("Não foi possível obter as recomendações de conselho de classe do aluno/turma", excecao.Message);
        }
    }
}