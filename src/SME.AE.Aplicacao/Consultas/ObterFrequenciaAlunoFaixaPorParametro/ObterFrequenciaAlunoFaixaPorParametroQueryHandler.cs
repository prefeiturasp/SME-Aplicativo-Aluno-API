using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterFrequenciaAlunoFaixaPorParametroQueryHandler : IRequestHandler<ObterFrequenciaAlunoFaixaPorParametroQuery, IEnumerable<FrequenciaAlunoFaixa>>
    {
        public ObterFrequenciaAlunoFaixaPorParametroQueryHandler()
        {
        }

        public async Task<IEnumerable<FrequenciaAlunoFaixa>> Handle(ObterFrequenciaAlunoFaixaPorParametroQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(request.Parametros
                .Select(x => new FrequenciaAlunoFaixa
                {
                    Faixa = decimal.TryParse(x.Conteudo, out var faixa) ? faixa : default,
                    Frequencia = x.Chave
                })
                .ToList());
        }
    }
}
