using MediatR;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQueryHandler : IRequestHandler<ObterDadosAlunosQuery, IEnumerable<AlunoRespostaEol>>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterDadosAlunosQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<AlunoRespostaEol>> Handle(ObterDadosAlunosQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
            var paramQueryDre = $"codigoDre={request.CodigoDre}";
            var paramQueryUe = $"codigoUe={request.CodigoUe}";
            var paramQueryAluno = $"codigoAluno={request.CodigoAluno}";
            var paramQueryResponsavel = $"cpfResponsavel={request.CpfResponsavel}";
            var url = $"alunos/dados-acompanhamento-escolar?{paramQueryResponsavel}&{paramQueryDre}&{paramQueryUe}&{paramQueryAluno}";

            var resposta = await httpClient.GetAsync(url);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<AlunoRespostaEol>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter dados do aluno");
            }
        }
    }
}
