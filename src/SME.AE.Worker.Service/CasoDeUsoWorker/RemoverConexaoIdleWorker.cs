using SME.AE.Aplicacao.CasoDeUso.AgendadoWorkerService;
using System;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "RemoverConexaoIdle", CronPadrao = "*/10 * * * *")]
    public class RemoverConexaoIdleWorker : UseCaseWorker<RemoverConexaoIdleCasoDeUso>
    {
        public RemoverConexaoIdleWorker(IServiceProvider serviceProvider)
            : base (serviceProvider)
        {
        }
    }
}
