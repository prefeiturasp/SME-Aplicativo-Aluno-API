using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterTotaisAdesaoAgrupadosPorDre
{
    public class ObterTotaisAdesaoAgrupadosPorDreQuery : IRequest<IEnumerable<TotaisAdesaoResultado>>
    {
        public ObterTotaisAdesaoAgrupadosPorDreQuery()
        {
        }
    }
}
