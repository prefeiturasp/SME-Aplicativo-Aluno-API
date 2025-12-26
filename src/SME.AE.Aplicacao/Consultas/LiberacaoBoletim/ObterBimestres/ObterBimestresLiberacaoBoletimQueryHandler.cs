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
            return [1, 2, 3, 4,0];
        }
    }
}

