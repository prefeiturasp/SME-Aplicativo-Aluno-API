using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterTurmasModalidadesPorCodigosQueryHandler : IRequestHandler<ObterTurmasModalidadesPorCodigosQuery, IEnumerable<TurmaModalidadeCodigoDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterTurmasModalidadesPorCodigosQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<IEnumerable<TurmaModalidadeCodigoDto>> Handle(ObterTurmasModalidadesPorCodigosQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");

            var turmasCodigos = string.Join("&turmasCodigo=", request.TurmaCodigo);

            var resposta = await httpClient.GetAsync($"v1/turma/integracoes/modalidades?turmasCodigo={turmasCodigos}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<TurmaModalidadeCodigoDto>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter as modalidades das turmas {turmasCodigos}");
            }            
        }
    }
}
