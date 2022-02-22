using MediatR;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class SolicitarBoletimAlunoQueryHendler : IRequestHandler<SolicitarBoletimAlunoQuery, bool>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public SolicitarBoletimAlunoQueryHendler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<bool> Handle(SolicitarBoletimAlunoQuery request, CancellationToken cancellationToken)
        {
            bool sucesso = false;
            var httpClient = httpClientFactory.CreateClient("servicoApiSgp");
            var body = JsonConvert.SerializeObject(request);
            var resposta = await httpClient.PostAsync($"v1/boletim/integracoes/imprimir", new StringContent(body, Encoding.UTF8, "application/json"));
            if (resposta.IsSuccessStatusCode)
            {
                sucesso = true;
            }
            else
                sucesso = false;
            return sucesso;
        }
    }
}
