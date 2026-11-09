using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    public class ObterFrequenciaGlobalAlunoQueryTeste
    {
        [Fact]
        public void Construtor_FactoryNula_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterFrequenciaGlobalAlunoQueryHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_RespostaSucessoComJson_DeveMapearFrequencia()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.OK, "95,5");
            var handler = new ObterFrequenciaGlobalAlunoQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterFrequenciaGlobalAlunoQuery("T1", "A1"), CancellationToken.None);

            Assert.Equal(95.5, resultado.Frequencia);
            Assert.Contains("alunos/A1/turmas/T1/geral", factory.Handler.UltimaRequisicao.RequestUri.ToString());
        }

        [Fact]
        public async Task Handle_RespostaSucessoSemJson_DeveRetornarFrequenciaZerada()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.OK, "\"\"");
            var handler = new ObterFrequenciaGlobalAlunoQueryHandler(factory.Factory.Object);

            var resultado = await handler.Handle(new ObterFrequenciaGlobalAlunoQuery("T1", "A1"), CancellationToken.None);

            Assert.Equal(0, resultado.Frequencia);
        }

        [Fact]
        public async Task Handle_RespostaNaoSucesso_DeveLancarExcecaoDeFrequenciaNaoEncontrada()
        {
            var factory = new HttpClientFactoryTeste(HttpStatusCode.NotFound, "", "Não encontrado");
            var handler = new ObterFrequenciaGlobalAlunoQueryHandler(factory.Factory.Object);

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(new ObterFrequenciaGlobalAlunoQuery("T1", "A1"), CancellationToken.None));

            Assert.Equal("A frequência global do aluno não foi encontrada", excecao.Message);
        }
    }
}