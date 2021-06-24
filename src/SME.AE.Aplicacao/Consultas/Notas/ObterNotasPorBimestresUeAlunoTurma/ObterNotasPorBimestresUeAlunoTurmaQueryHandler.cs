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
    public class ObterNotasPorBimestresUeAlunoTurmaQueryHandler : IRequestHandler<ObterNotasPorBimestresUeAlunoTurmaQuery, IEnumerable<NotaConceitoBimestreComponenteDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterNotasPorBimestresUeAlunoTurmaQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> Handle(ObterNotasPorBimestresUeAlunoTurmaQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<NotaConceitoBimestreComponenteDto> notasConceitos;

            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"v1/avaliacoes/notas/ues/{request.UeId}/turmas/{request.TurmaId}/alunos/{request.AlunoCodigo}?bimestres={string.Join("&bimestres=", request.Bimestres)}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                notasConceitos = JsonConvert.DeserializeObject<IEnumerable<NotaConceitoBimestreComponenteDto>>(json);
            }
            else
            {
                SentrySdk.CaptureMessage(resposta.ReasonPhrase);
                throw new Exception();
            }

            return notasConceitos;
        }
    }
}
