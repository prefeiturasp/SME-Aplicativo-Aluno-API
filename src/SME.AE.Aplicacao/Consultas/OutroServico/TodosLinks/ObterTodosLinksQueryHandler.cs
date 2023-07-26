using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterTodosLinksQueryHandler : IRequestHandler<ObterTodosLinksQuery, IEnumerable<OutroServicoDto>>
    {
        private readonly IOutroServicoRepositorio outroServicoRepositorio;
        public ObterTodosLinksQueryHandler(IOutroServicoRepositorio outroServicoRepositorio)
        {
            this.outroServicoRepositorio = outroServicoRepositorio ?? throw new System.ArgumentNullException(nameof(outroServicoRepositorio));
        }
        public async Task<IEnumerable<OutroServicoDto>> Handle(ObterTodosLinksQuery request, CancellationToken cancellationToken)
        {
            return await outroServicoRepositorio.Links();
        }
    }
}
