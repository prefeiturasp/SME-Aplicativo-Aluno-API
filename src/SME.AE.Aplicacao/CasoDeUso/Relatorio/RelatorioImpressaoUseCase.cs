using MediatR;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class RelatorioImpressaoUseCase : IRelatorioImpressaoUseCase
    {
        private readonly IMediator mediator;

        public RelatorioImpressaoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async  Task<bool> Executar(Guid codigo)
        {
            return await mediator.Send(new ConsultarSeRelatorioExisteQuery(codigo));
        }
    }
}
