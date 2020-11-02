using SME.AE.Aplicacao.CasoDeUso;
using System;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "TransferirNotaSgp", CronPadrao = "0 14,22 * * *")]
    public class TransferirNotaSgpWorker : UseCaseWorker<TransferirNotaSgpCasoDeUso>
    {
        public TransferirNotaSgpWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
