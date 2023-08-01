using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterResponsaveisPorDreEUeQueryHandler : IRequestHandler<ObterResponsaveisPorDreEUeQuery, IEnumerable<ResponsavelAlunoEOLDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ObterResponsaveisPorDreEUeQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<ResponsavelAlunoEOLDto>> Handle(ObterResponsaveisPorDreEUeQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
            var paramQueryDre = $"codigoDre={request.CodigoDre}";
            var paramQueryUe = $"codigoUe={request.CodigoUe}";
            var paramQueryAnoLetivo = $"anoLetivo={request.AnoLetivo}";
            var url = $"alunos/responsaveis?{paramQueryDre}&{paramQueryUe}&{paramQueryAnoLetivo}";

            var resposta = await httpClient.GetAsync(url);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var cpfsDeResponsavel = JsonConvert.DeserializeObject<IEnumerable<ResponsavelAlunoEOLDto>>(json);
                if (cpfsDeResponsavel == null || !cpfsDeResponsavel.Any())
                    throw new NegocioException("Não existem registros de responsáveis para a DRE e UE informadas.");
                return cpfsDeResponsavel;
            }
            else
            {
                throw new System.Exception($"Não foi possível obter registros de responsáveis para a DRE e UE informadas.");
            }
        }
    }
}
