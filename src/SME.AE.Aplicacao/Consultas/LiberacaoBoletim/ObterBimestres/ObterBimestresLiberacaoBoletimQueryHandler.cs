using MediatR;
using Newtonsoft.Json;
using Sentry;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterBimestres
{
    public class ObterBimestresLiberacaoBoletimQueryHandler : IRequestHandler<ObterBimestresLiberacaoBoletimQuery, int[]>
    {

        private readonly IHttpClientFactory httpClientFactory;

        public ObterBimestresLiberacaoBoletimQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<int[]> Handle(ObterBimestresLiberacaoBoletimQuery request, CancellationToken cancellationToken)
        {
            int[] bimestres;
            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var resposta = await httpClient.GetAsync($"api/v1/calendarios/eventos/liberacao-boletim/turmas/{request.TurmaCodigo}/bimestres");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                bimestres = JsonConvert.DeserializeObject<int[]>(json);
            }
            else
            {
                throw new Exception($"Não foi possível localizar os bimetres da liberação de boletim da turma do aluno.");
            }

            return bimestres;
        }
    }
}

