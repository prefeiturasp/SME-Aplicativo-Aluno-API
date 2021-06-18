using MediatR;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class EnviarAtualizacaoCadastralProdamCommandHandler : IRequestHandler<EnviarAtualizacaoCadastralProdamCommand, bool>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public EnviarAtualizacaoCadastralProdamCommandHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<bool> Handle(EnviarAtualizacaoCadastralProdamCommand request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoAtualizacaoCadastralProdam");

            var filtro = JsonConvert.SerializeObject(request.ResponsavelDto);
            var resposta = await httpClient.PostAsync($"AtualizarResponsavelAluno", new StringContent(filtro, Encoding.UTF8, "application/json-patch+json"));

            if (resposta.IsSuccessStatusCode && resposta.StatusCode != HttpStatusCode.NoContent)
                return true;

            return false;
        }
    }
}
