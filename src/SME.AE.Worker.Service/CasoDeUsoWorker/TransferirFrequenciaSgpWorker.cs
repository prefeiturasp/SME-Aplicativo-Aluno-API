using SME.AE.Aplicacao.CasoDeUso;
using System;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "TransferirFrequenciaSgp", CronPadrao = "0 14,22 * * *")]
    public class TransferirFrequenciaSgpWorker : UseCaseWorker<TransferirFrequenciaSgpCasoDeUso>
    {
        public TransferirFrequenciaSgpWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
