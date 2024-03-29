﻿using MediatR;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
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
            var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
            var resposta = await httpClient.GetAsync($"v1/calendarios/frequencias/integracoes/alunos/{request.AlunoCodigo}/turmas/{request.TurmaCodigo}/geral");
            if (resposta.IsSuccessStatusCode)
            {
                frequenciaGlobal = new FrequenciaGlobalDto();
                var json = await resposta.Content.ReadAsStringAsync();
                frequenciaGlobal.Frequencia = JsonConvert.DeserializeObject<double>(json.Replace(',', '.'));
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
