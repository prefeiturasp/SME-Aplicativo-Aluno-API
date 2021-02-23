using SME.AE.Aplicacao.CasoDeUso;
using System;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "ConsolidarAdesaoEOL", CronPadrao = "0 7,14 * * *")]
    public class ConsolidarAdesaoEOLWorker : UseCaseWorker<ConsolidarAdesaoEOLCasoDeUso>
    {
        public ConsolidarAdesaoEOLWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
