using SME.AE.Aplicacao.CasoDeUso;
using System;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "TransferirEventosSgp", CronPadrao = "0 0 * * *")]
    public class TransferirEventoSgpWorker : UseCaseWorker<TranferirEventoSgpCasoDeUso>
    {
        public TransferirEventoSgpWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
