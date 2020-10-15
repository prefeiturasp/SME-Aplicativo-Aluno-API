using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Consultas.ObterTotalUsuariosComAcessoIncompleto;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterTotalUsuariosComAcessoIncompletoUseCase : IObterTotalUsuariosComAcessoIncompletoUseCase
    {
        private readonly IMediator mediator;

        public ObterTotalUsuariosComAcessoIncompletoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Executar(string codigoDre, string codigoUe)
        {
            return await mediator.Send(new ObterTotalUsuariosComAcessoIncompletoQuery(codigoDre, codigoUe));
        }
    }
}
