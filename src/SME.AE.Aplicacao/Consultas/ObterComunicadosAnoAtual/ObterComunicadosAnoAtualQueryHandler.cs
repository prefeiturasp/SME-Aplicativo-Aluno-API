using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterComunicadosAnoAtualQueryHandler : IRequestHandler<ObterComunicadosAnoAtualQuery, IEnumerable<ComunicadoSgpDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ObterComunicadosAnoAtualQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<IEnumerable<ComunicadoSgpDto>> Handle(ObterComunicadosAnoAtualQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
            var resposta = await httpClient.GetAsync($"v1/comunicados/integracoes/ano-atual", cancellationToken);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<ComunicadoSgpDto>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter os comunicados dos alunos/turmas para o ano atual");
            }
        }
    }
}
