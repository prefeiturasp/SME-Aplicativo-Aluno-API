using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
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
    public class ObterRecomendacoesPorAlunosTurmasQueryHandler : IRequestHandler<ObterRecomendacoesPorAlunosTurmasQuery, IEnumerable<RecomendacaoConselhoClasseAluno>>
    {
        private readonly IHttpClientFactory httpClientFactory;
        public ObterRecomendacoesPorAlunosTurmasQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }
        public async Task<IEnumerable<RecomendacaoConselhoClasseAluno>> Handle(ObterRecomendacoesPorAlunosTurmasQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiSgpChave");
            var paramQueryAluno = $"codigoAluno={request.CodigoAluno}";
            var paramQueryTurma = $"codigoTurma={request.CodigoTurma}";
            var paramQueryAnoLetivo = $"anoLetivo={request.AnoLetivo}";
            var paramQueryModalidade = $"modalidade={(int)request.Modalidade}";
            var paramQuerySemestre = $"semestre={request.Semestre}";

            var resposta = await httpClient.GetAsync($"v1/conselhos-classe/recomendacoes/integracoes/alunos?{paramQueryAluno}&{paramQueryTurma}&{paramQueryAnoLetivo}&{paramQueryModalidade}&{paramQuerySemestre}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<RecomendacaoConselhoClasseAluno>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter as recomendações de conselho de classe do aluno/turma");
            }
        }
    }
}
