using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterFrequenciaAlunoCorPorParametroQueryHandler : IRequestHandler<ObterFrequenciaAlunoCorPorParametroQuery, IEnumerable<FrequenciaAlunoCor>>
    {
        public ObterFrequenciaAlunoCorPorParametroQueryHandler()
        {
        }

        public async Task<IEnumerable<FrequenciaAlunoCor>> Handle(ObterFrequenciaAlunoCorPorParametroQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(request.Parametros
               .Select(x => new FrequenciaAlunoCor
               {
                   Cor = x.Conteudo,
                   Frequencia = x.Chave
               })
               .ToList());
        }
    }
}
