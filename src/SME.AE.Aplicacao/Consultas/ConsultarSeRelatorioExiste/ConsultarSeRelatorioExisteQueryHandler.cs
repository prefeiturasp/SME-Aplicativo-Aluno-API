using MediatR;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ConsultarSeRelatorioExisteQueryHandler : IRequestHandler<ConsultarSeRelatorioExisteQuery, bool>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ConsultarSeRelatorioExisteQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<bool> Handle(ConsultarSeRelatorioExisteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                bool existe = false;
                var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
                
                var resposta = await httpClient.GetAsync($"v1/relatorios/integracoes/existe?codigoRelatorio={request.CodigoCorrelacao}");
                if (resposta.IsSuccessStatusCode)
                {
                    var json = await resposta.Content.ReadAsStringAsync();
                    existe = json != null && JsonConvert.DeserializeObject<bool>(json);
                }
                return existe;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
