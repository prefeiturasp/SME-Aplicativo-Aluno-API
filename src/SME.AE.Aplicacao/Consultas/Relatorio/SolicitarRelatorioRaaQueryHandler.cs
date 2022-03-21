using MediatR;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class SolicitarRelatorioRaaQueryHandler : IRequestHandler<SolicitarRelatorioRaaQuery, bool>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public SolicitarRelatorioRaaQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<bool> Handle(SolicitarRelatorioRaaQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var body = JsonConvert.SerializeObject(request);
            var resposta = await httpClient.PostAsync($"v1/relatorios/integracoes/raa", new StringContent(body, Encoding.UTF8, "application/json"));
            bool sucesso;
            if (resposta.IsSuccessStatusCode)
                sucesso = true;
            else
                sucesso = false;
            return sucesso;
        }
    }
}
