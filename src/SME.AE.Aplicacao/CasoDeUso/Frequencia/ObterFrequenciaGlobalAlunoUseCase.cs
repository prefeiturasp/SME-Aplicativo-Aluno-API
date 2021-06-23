using MediatR;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciaGlobalAlunoUseCase : IObterFrequenciaGlobalAlunoUseCase
    {
        private readonly IMediator mediator;

        public ObterFrequenciaGlobalAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<double?> Executar(FiltroFrequenciaGlobalAlunoDto filtro)
        {
            var frequenciaGlobal = await mediator.Send(new ObterFrequenciaGlobalAlunoQuery(filtro.TurmaCodigo, filtro.AlunoCodigo));

            return frequenciaGlobal;
        }
    }
}
