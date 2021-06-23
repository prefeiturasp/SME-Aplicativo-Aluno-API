using MediatR;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciaGlobalAlunoQueryHandler : IRequestHandler<ObterFrequenciaGlobalAlunoQuery, double?>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterFrequenciaGlobalAlunoQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<double?> Handle(ObterFrequenciaGlobalAlunoQuery request, CancellationToken cancellationToken)
        {
            double frequenciaGlobal;
            var httpClient = httpClientFactory.CreateClient("servicoAtualizacaoCadastral");
            var resposta = await httpClient.GetAsync("palavras-bloqueadas");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                frequenciaGlobal = JsonConvert.DeserializeObject<double>(json);
            }
            else
            {
                SentrySdk.CaptureMessage(resposta.ReasonPhrase);
                return null;
            }

            return frequenciaGlobal;
        }
    }
}
