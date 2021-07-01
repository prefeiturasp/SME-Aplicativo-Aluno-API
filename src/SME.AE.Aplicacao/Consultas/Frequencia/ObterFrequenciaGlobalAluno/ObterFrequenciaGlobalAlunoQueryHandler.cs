using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciaGlobalAlunoQueryHandler : IRequestHandler<ObterFrequenciaGlobalAlunoQuery, FrequenciaGlobalDto>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterFrequenciaGlobalAlunoQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<FrequenciaGlobalDto> Handle(ObterFrequenciaGlobalAlunoQuery request, CancellationToken cancellationToken)
        {
            FrequenciaGlobalDto frequenciaGlobal;
            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"v1/calendarios/frequencias/alunos/{request.AlunoCodigo}/turmas/{request.TurmaCodigo}/geral");
            if (resposta.IsSuccessStatusCode)
            {
                frequenciaGlobal = new FrequenciaGlobalDto();
                var json = await resposta.Content.ReadAsStringAsync();
                frequenciaGlobal.Frequencia = JsonConvert.DeserializeObject<double>(json);
            }
            else
            {
                SentrySdk.CaptureMessage(resposta.ReasonPhrase);
                throw new Exception("A frequência global do aluno não foi encontrada");
            }

            return frequenciaGlobal;
        }
    }
}
