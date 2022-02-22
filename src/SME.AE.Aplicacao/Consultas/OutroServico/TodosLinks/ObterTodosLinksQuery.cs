using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao
{
    public class ObterTodosLinksQuery : IRequest<IEnumerable<OutroServicoDto>> { }
}
