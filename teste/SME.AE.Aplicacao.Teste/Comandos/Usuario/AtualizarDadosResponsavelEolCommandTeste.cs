using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SME.AE.Aplicacao;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class AtualizarDadosResponsavelEolCommandTeste
    {
        [Fact]
        public void Construtor_ComFactoryNula_DeveLancarExcecao()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new AtualizarDadosResponsavelEolCommandHandler(null));

            Assert.Equal("httpClientFactory", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_ComRespostaDeSucessoEConteudoValido_DeveRetornarVerdadeiro()
        {
            var requisicoes = 0;
            var cliente = CriarCliente((request, _) =>
            {
                requisicoes++;
                Assert.Equal(HttpMethod.Post, request.Method);
                Assert.Equal("/alunos/aluno-1/responsaveis/123", request.RequestUri.PathAndQuery);
                return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("true") };
            });
            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient("servicoApiEolChave")).Returns(cliente);
            var handler = new AtualizarDadosResponsavelEolCommandHandler(factory.Object);

            var resultado = await handler.Handle(CriarCommand(), CancellationToken.None);

            Assert.True(resultado);
            Assert.Equal(1, requisicoes);
        }

        [Fact]
        public async Task Handle_ComRespostaNoContent_DeveLancarExcecao()
        {
            var handler = CriarHandler(HttpStatusCode.NoContent, "true");

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(CriarCommand(), CancellationToken.None));

            Assert.Contains("Não foi possível atualizar", excecao.Message);
        }

        [Fact]
        public async Task Handle_ComRespostaHttpDeFalha_DeveLancarExcecao()
        {
            var handler = CriarHandler(HttpStatusCode.BadRequest, "false");

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(CriarCommand(), CancellationToken.None));

            Assert.Contains("Não foi possível atualizar", excecao.Message);
        }

        [Fact]
        public async Task Handle_ComRespostaDeSucessoContendoFalse_DeveLancarExcecaoComRetorno()
        {
            var handler = CriarHandler(HttpStatusCode.OK, "{\"sucesso\": false}");

            var excecao = await Assert.ThrowsAsync<Exception>(() => handler.Handle(CriarCommand(), CancellationToken.None));

            Assert.Contains("Retorno:", excecao.Message);
            Assert.Contains("false", excecao.Message);
        }

        private static AtualizarDadosResponsavelEolCommandHandler CriarHandler(HttpStatusCode statusCode, string conteudo)
        {
            var factory = new Mock<IHttpClientFactory>();
            factory.Setup(x => x.CreateClient("servicoApiEolChave"))
                .Returns(CriarCliente((_, _) => new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(conteudo)
                }));
            return new AtualizarDadosResponsavelEolCommandHandler(factory.Object);
        }

        private static HttpClient CriarCliente(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> resposta)
        {
            return new HttpClient(new DelegatingHandlerDeTeste(resposta))
            {
                BaseAddress = new Uri("https://teste.local/")
            };
        }

        private static AtualizarDadosResponsavelEolCommand CriarCommand()
        {
            return new AtualizarDadosResponsavelEolCommand(
                "aluno-1", 123, "email@teste.com", new DateTime(2000, 1, 2), "Mãe", "999999999", "11");
        }

        private sealed class DelegatingHandlerDeTeste : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> resposta;

            public DelegatingHandlerDeTeste(Func<HttpRequestMessage, CancellationToken, HttpResponseMessage> resposta)
            {
                this.resposta = resposta;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(resposta(request, cancellationToken));
            }
        }
    }
}
