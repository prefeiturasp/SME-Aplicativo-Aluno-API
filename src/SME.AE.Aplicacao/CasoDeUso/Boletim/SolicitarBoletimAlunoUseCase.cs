using MediatR;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class SolicitarBoletimAlunoUseCase : ISolicitarBoletimAlunoUseCase
    {
        private readonly IMediator mediator;

        public SolicitarBoletimAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(SolicitarBoletimAlunoDto solicitarBoletim)
        {
            return await mediator.Send(new SolicitarBoletimAlunoQuery(solicitarBoletim.DreCodigo, solicitarBoletim.UeCodigo, 
                solicitarBoletim.Semestre, solicitarBoletim.TurmaCodigo, solicitarBoletim.AnoLetivo, solicitarBoletim.ModalidadeCodigo, solicitarBoletim.Modelo, solicitarBoletim.AlunosCodigo));
        }
    }
}
