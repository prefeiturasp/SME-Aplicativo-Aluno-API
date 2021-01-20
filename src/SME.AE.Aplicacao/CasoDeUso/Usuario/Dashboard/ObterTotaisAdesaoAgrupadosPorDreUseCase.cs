using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterTotaisAdesaoAgrupadosPorDre;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterTotaisAdesaoAgrupadosPorDreUseCase : IObterTotaisAdesaoAgrupadosPorDreUseCase
    {
        private readonly IMediator mediator;

        public ObterTotaisAdesaoAgrupadosPorDreUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<TotaisAdesaoResultado>> Executar()
        {
            return await mediator.Send(new ObterTotaisAdesaoAgrupadosPorDreQuery());
        }
    }
}
