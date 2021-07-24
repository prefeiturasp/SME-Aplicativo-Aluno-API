using MediatR;
using Newtonsoft.Json;
using Sentry;
using System;
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
            var resposta = await httpClient.GetAsync($"v1/turmas/modalidades?turmasCodigo={turmasCodigos}");
            var json = "";
            try
            {
                if (resposta.IsSuccessStatusCode)
                {
                    json = await resposta.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<TurmaModalidadeCodigoDto>>(json);
                }
            }
            catch(Exception ex)
            {
                SentrySdk.CaptureException(ex);
                SentrySdk.CaptureMessage($"ObterTurmasModalidadesPorCodigosQueryHandler {json}");
                throw new System.Exception($"Não foi possível obter as modalidades das turmas {turmasCodigos}");
            }
            return null;
        }
    }
}
