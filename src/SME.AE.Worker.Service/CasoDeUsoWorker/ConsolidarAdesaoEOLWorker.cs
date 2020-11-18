using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

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
