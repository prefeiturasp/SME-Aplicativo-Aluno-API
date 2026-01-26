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
                var existe = false;
                var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");

                var resposta = await httpClient.GetAsync($"v1/relatorios/integracoes/existe?codigoRelatorio={request.CodigoCorrelacao}", cancellationToken);
                if (!resposta.IsSuccessStatusCode) return false;
                var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
                existe = JsonConvert.DeserializeObject<bool>(json);
                return existe;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
