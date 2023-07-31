using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Sentry;
using System;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Servicos
{
    public class ServicoLog : IServicoLog
    {
        private readonly string sentryDSN;

        public ServicoLog(IConfiguration configuration, TelemetryClient insightsClient)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            sentryDSN = configuration.GetSection("Sentry:DSN").Value;
        }

        public void Registrar(string mensagem)
        {
            using (SentrySdk.Init(sentryDSN))
            {
                SentrySdk.CaptureMessage(mensagem);
            }
        }

        public void Registrar(Exception ex)
        {
            using (SentrySdk.Init(sentryDSN))
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
