using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class OutroServicoUseCase : IOutroServicoUseCase
    {

        private readonly IMediator mediator;
        public OutroServicoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<IEnumerable<OutroServicoDto>> Links()
        {
            return await mediator.Send(new ObterTodosLinksQuery());
        }

        public async Task<IEnumerable<OutroServicoDto>> LinksPrioritarios()
        {
            return await mediator.Send(new ObterListaLinksPrioriatiosQuery());
        }
    }
}
