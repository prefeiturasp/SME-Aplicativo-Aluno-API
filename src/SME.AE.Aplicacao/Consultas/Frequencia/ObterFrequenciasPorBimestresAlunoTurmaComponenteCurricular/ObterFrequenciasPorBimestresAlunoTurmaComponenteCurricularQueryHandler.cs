using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler : IRequestHandler<ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery, IEnumerable<FrequenciaAlunoDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<FrequenciaAlunoDto>> Handle(ObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FrequenciaAlunoDto> frequencias;

            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"v1/calendarios/frequencias/turmas/{request.TurmaCodigo}/alunos/{request.AlunoCodigo}/componentes-curriculares/{request.ComponenteCurricularId}?bimestres={string.Join("&bimestres=", request.Bimestres)}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                frequencias = JsonConvert.DeserializeObject<IEnumerable<FrequenciaAlunoDto>>(json);
            }
            else
            {
                throw new Exception($"Não foi possível localizar as frequências do aluno : {request.AlunoCodigo} da turma {request.TurmaCodigo}.");
            }

            return frequencias;
        }
    }
}
