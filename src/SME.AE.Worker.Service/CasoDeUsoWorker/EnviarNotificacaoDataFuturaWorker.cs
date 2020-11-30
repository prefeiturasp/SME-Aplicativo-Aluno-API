using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(CronParametroDB = "EnviarNotificacaoDataFutura", CronPadrao = "0 7 * * *")]
    public class EnviarNotificacaoDataFuturaWorker : UseCaseWorker<EnviarNotificacaoDataFuturaCasoDeUso>
    {
        public EnviarNotificacaoDataFuturaWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
