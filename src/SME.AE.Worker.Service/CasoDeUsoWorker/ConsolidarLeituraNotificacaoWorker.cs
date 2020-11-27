using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "ConsolidarLeituraNotificacao", CronPadrao = "0 8 * * *")]
    public class ConsolidarLeituraNotificacaoWorker : UseCaseWorker<ConsolidarLeituraNotificacaoCasoDeUso>
    {
        public ConsolidarLeituraNotificacaoWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
