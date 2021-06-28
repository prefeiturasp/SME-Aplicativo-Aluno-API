using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterTurmasModalidadesPorCodigosQueryHandler : IRequestHandler<ObterTurmasModalidadesPorCodigosQuery, IEnumerable<TurmaModalidadeCodigoDto>>
    {
        public ObterTurmasModalidadesPorCodigosQueryHandler()
        {

        }
        public Task<IEnumerable<TurmaModalidadeCodigoDto>> Handle(ObterTurmasModalidadesPorCodigosQuery request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
