using Moq;
using System;
using System.Net.Http;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Teste.Consultas.Aluno
{
    internal sealed class HttpClientFactoryTeste
    {
        public HttpClientFactoryTeste(HttpStatusCode statusCode, string content, string reasonPhrase = "erro")
        {
            Handler = new HandlerTeste(_ => new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content ?? string.Empty),
                ReasonPhrase = reasonPhrase
            });
            Client = new HttpClient(Handler) { BaseAddress = new Uri("https://teste.local/") };
            Factory = new Mock<IHttpClientFactory>();
            Factory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(Client);
        }

        public Mock<IHttpClientFactory> Factory { get; }
        public HttpClient Client { get; }
        public HandlerTeste Handler { get; }

        internal sealed class HandlerTeste : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, HttpResponseMessage> responder;

            public HandlerTeste(Func<HttpRequestMessage, HttpResponseMessage> responder)
            {
                this.responder = responder;
            }

            public HttpRequestMessage UltimaRequisicao { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                UltimaRequisicao = request;
                return Task.FromResult(responder(request));
            }
        }
    }
}