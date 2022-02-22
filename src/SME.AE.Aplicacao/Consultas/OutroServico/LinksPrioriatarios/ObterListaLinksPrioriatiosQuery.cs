using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao
{
    public class ObterListaLinksPrioriatiosQuery : IRequest<IEnumerable<OutroServicoDto>>
    {
        public ObterListaLinksPrioriatiosQuery()
        {

        }
    }
}
