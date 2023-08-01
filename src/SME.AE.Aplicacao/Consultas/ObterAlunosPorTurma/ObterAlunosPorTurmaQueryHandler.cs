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
    public class ObterAlunosPorTurmaQueryHandler : IRequestHandler<ObterAlunosPorTurmaQuery, IEnumerable<AlunoTurmaEol>>
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ObterAlunosPorTurmaQueryHandler(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory ?? throw new System.ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<IEnumerable<AlunoTurmaEol>> Handle(ObterAlunosPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var httpClient = httpClientFactory.CreateClient("servicoApiEolChave");
            var url = $"turmas/{request.CodigoTurma}/acompanhamento-escolar/todos-alunos";

            var resposta = await httpClient.GetAsync(url);
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<AlunoTurmaEol>>(json);
            }
            else
            {
                throw new System.Exception($"Não foi possível obter os alunos da turma");
            }
        }
    }
}
