using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterTotalUsuariosValidosUseCase : IObterTotalUsuariosValidosUseCase
    {
        private readonly IMediator mediator;

        public ObterTotalUsuariosValidosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Executar(string codigoDre, string codigoUe)
        {
            return await mediator.Send(new ObterTotalUsuariosValidosQuery(codigoDre, codigoUe));
        }
    }
}
