using MediatR;
using Newtonsoft.Json;
using System;
using System.Net.Http;
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
            var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
            var resposta = await httpClient.GetAsync($"v1/calendarios/eventos/integracoes/liberacao-boletim/turmas/{request.TurmaCodigo}/bimestres");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                bimestres = JsonConvert.DeserializeObject<int[]>(json);
                if (json.Contains("-99"))
                    bimestres = [1, 2, 3, 4, 0];
            }
            else
            {
                return [1, 2, 3, 4];
            }

            return bimestres;
        }
    }
}

