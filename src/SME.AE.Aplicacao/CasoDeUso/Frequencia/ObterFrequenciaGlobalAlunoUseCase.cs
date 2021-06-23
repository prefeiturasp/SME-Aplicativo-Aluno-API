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

        public async Task<bool> Executar(FiltroFrequenciaGlobalAlunoDto filtro)
        {
            throw new NotImplementedException();
        }
    }
}
