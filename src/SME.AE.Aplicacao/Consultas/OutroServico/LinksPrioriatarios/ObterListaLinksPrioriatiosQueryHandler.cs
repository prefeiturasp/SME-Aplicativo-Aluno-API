using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.OutroServico
{
    public class ObterListaLinksPrioriatiosQueryHandler : IRequestHandler<ObterListaLinksPrioriatiosQuery, IEnumerable<OutroServicoDto>>
    {
        private readonly IOutroServicoRepositorio outroServicoRepositorio;
        public ObterListaLinksPrioriatiosQueryHandler(IOutroServicoRepositorio outroServicoRepositorio)
        {
            this.outroServicoRepositorio = outroServicoRepositorio ?? throw new System.ArgumentNullException(nameof(outroServicoRepositorio));
        }
        public async Task<IEnumerable<OutroServicoDto>> Handle(ObterListaLinksPrioriatiosQuery request, CancellationToken cancellationToken)
        {
            return await outroServicoRepositorio.LinksPrioritarios();
        }
    }
}
