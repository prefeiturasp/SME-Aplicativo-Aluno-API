using SME.AE.Aplicacao.CasoDeUso.AgendadoWorkerService;
using System;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    public class RemoverConexaoIdleWorker : UseCaseWorker<RemoverConexaoIdleCasoDeUso>
    {
        public RemoverConexaoIdleWorker(IServiceProvider serviceProvider)
            : base (serviceProvider)
        {
        }
    }
}
