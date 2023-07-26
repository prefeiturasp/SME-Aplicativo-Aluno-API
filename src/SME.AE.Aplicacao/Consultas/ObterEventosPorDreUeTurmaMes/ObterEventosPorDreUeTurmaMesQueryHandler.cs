using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterRecomendacoesPorAlunosTurmas
{
    public class ObterEventosPorDreUeTurmaMesQueryHandler : IRequestHandler<ObterEventosPorDreUeTurmaMesQuery, IEnumerable<EventoDto>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ObterEventosPorDreUeTurmaMesQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<IEnumerable<EventoDto>> Handle(ObterEventosPorDreUeTurmaMesQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
            var paramQueryDre = $"codigoDre={request.Dre_id}";
            var paramQueryUe = $"codigoUe={request.Ue_id}";
            var paramQueryTurma = $"codigoTurma={request.Turma_id}";
            
            var resposta = await httpClient.GetAsync($"v1/calendarios/eventos/integracoes/modalidadesCalendario/{request.ModalidadeCalendario}/mesAno/{request.MesAno:yyyy-MM-dd}?{paramQueryDre}&{paramQueryUe}&{paramQueryTurma}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<EventoDto>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter os eventos da dre/ue");
            }
        }
    }
}
