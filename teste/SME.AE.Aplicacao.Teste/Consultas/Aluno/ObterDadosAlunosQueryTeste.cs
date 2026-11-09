using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    public class ObterDadosAlunosQueryTeste
    {
        [Fact]
        public void Construtor_FactoryNula_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterDadosAlunosQueryHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_RespostaSucesso_DeveDesserializarEEnviarParametros()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.OK, "[{\"CodigoEol\":123,\"Nome\":\"Aluno\"}]");
            var handler = new ObterDadosAlunosQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterDadosAlunosQuery("12345678901", 10, "D1", "U1"), CancellationToken.None);

            var aluno = Assert.Single(resultado);
            Assert.Equal(123, aluno.CodigoEol);
            Assert.Equal("Aluno", aluno.Nome);
            Assert.Contains("cpfResponsavel=12345678901", factory.Handler.UltimaRequisicao.RequestUri.ToString());
            Assert.Contains("codigoDre=D1", factory.Handler.UltimaRequisicao.RequestUri.ToString());
            Assert.Contains("codigoUe=U1", factory.Handler.UltimaRequisicao.RequestUri.ToString());
            Assert.Contains("codigoAluno=10", factory.Handler.UltimaRequisicao.RequestUri.ToString());
        }

        [Fact]
        public async Task Handle_RespostaNaoSucesso_DeveRetornarNulo()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.NotFound, "");
            var handler = new ObterDadosAlunosQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterDadosAlunosQuery(null, null, null, null), CancellationToken.None);

            Assert.Null(resultado);
        }
    }
}