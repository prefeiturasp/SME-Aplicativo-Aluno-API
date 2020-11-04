using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

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
