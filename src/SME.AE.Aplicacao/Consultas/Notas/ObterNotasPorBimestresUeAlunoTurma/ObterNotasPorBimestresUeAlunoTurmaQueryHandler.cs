using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterNotasPorBimestresUeAlunoTurmaQueryHandler : IRequestHandler<ObterNotasPorBimestresUeAlunoTurmaQuery, IEnumerable<NotaConceitoBimestreComponenteDto>>
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ObterNotasPorBimestresUeAlunoTurmaQueryHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<NotaConceitoBimestreComponenteDto>> Handle(ObterNotasPorBimestresUeAlunoTurmaQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<NotaConceitoBimestreComponenteDto> notasConceitos;

            var httpClient = _httpClientFactory.CreateClient("servicoApiSgpChave");
            var resposta = await httpClient.GetAsync($"v1/avaliacoes/notas/integracoes/ues/{request.UeCodigo}/turmas/{request.TurmaCodigo}/alunos/{request.AlunoCodigo}?bimestres={string.Join("&bimestres=", request.Bimestres)}", cancellationToken);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync(cancellationToken);
                notasConceitos = JsonConvert.DeserializeObject<IEnumerable<NotaConceitoBimestreComponenteDto>>(json);
            }
            else
            {
                throw new Exception($"Não foi possível localizar as notas do aluno : {request.AlunoCodigo} da turma {request.TurmaCodigo}.");
            }

            return notasConceitos;
        }



    }
}
