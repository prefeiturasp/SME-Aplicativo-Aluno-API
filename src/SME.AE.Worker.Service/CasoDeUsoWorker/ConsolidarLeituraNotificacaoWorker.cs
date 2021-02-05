using SME.AE.Aplicacao.CasoDeUso;
using System;

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
