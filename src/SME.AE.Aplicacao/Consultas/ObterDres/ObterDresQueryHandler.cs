using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.Dre;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterDresQueryHandler : IRequestHandler<ObterDresQuery, IEnumerable<DreResposta>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ObterDresQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<IEnumerable<DreResposta>> Handle(ObterDresQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
            var resposta = await httpClient.GetAsync($"v1/dres/integracoes", cancellationToken);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<DreResposta>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter as dres");
            }
        }
    }
}
