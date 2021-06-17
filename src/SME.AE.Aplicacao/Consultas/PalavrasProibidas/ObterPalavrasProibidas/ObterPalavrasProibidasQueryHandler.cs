using MediatR;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterPalavrasProibidasQueryHandler : IRequestHandler<ObterPalavrasProibidasQuery, string[]>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterPalavrasProibidasQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<string[]> Handle(ObterPalavrasProibidasQuery request, CancellationToken cancellationToken)
        {
            var palavrasBloqueadas = new string[] { };
            var httpClient = httpClientFactory.CreateClient("servicoAtualizacaoCadastral");            
            var resposta = await httpClient.GetAsync("palavras-bloqueadas");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                palavrasBloqueadas = JsonConvert.DeserializeObject<string[]>(json);
            }
            else
            {
                SentrySdk.CaptureMessage(resposta.ReasonPhrase);
                return null;
            }
            return palavrasBloqueadas;
        }
    }
}
