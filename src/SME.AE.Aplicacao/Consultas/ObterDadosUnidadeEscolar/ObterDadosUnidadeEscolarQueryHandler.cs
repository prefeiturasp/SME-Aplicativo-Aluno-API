using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.UnidadeEscolar
{
    public class ObterDadosUnidadeEscolarQueryHandler : IRequestHandler<ObterDadosUnidadeEscolarQuery, UnidadeEscolarResposta>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ObterDadosUnidadeEscolarQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<UnidadeEscolarResposta> Handle(ObterDadosUnidadeEscolarQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
            var resposta = await httpClient.GetAsync($"escolas/dados/{request.CodigoUe}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UnidadeEscolarResposta>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter os dados da escola");
            }

        }
    }
}