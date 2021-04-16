using SME.AE.Aplicacao.CasoDeUso;
using System;

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
