using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Worker.Service.CasoDeUsoWorker
{
    [UseCaseWorker(ParametroEscolaAqui = "TransferirEventosSgp", ParametroPadrao = "0 0 * * *")]
    public class TransferirEventoSgpWorker : UseCaseWorker<TranferirEventoSgpCasoDeUso>
    {
        public TransferirEventoSgpWorker(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }

}
