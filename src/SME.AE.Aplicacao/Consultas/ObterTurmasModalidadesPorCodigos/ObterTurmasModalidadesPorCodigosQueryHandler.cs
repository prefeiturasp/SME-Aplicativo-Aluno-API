using System;
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
                var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
                var turmasCodigos = string.Join("&turmasCodigo=", request.TurmaCodigo);
                var url = $"v1/turma/integracoes/modalidades?turmasCodigo={turmasCodigos}";
                var resposta = await httpClient.GetAsync(url,cancellationToken);
                if (!resposta.IsSuccessStatusCode) return null;
                var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
                return JsonConvert.DeserializeObject<IEnumerable<TurmaModalidadeCodigoDto>>(json);

        }
    }
}
