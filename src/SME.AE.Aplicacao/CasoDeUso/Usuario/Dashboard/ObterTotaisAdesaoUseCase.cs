using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterTotaisAdesao;
using SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterTotaisAdesaoUseCase : IObterTotaisAdesaoUseCase
    {
        private readonly IMediator mediator;

        public ObterTotaisAdesaoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> Executar(string codigoDre, string codigoUe)
        {
            return await mediator.Send(new ObterTotaisAdesaoQuery(codigoDre, codigoUe));
        }
    }    
}
