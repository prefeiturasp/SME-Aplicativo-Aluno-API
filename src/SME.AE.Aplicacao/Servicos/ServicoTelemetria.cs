﻿using Elastic.Apm;
using Microsoft.ApplicationInsights;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Dominio.Options;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Servicos
{
    public class ServicoTelemetria : IServicoTelemetria
    {
        private readonly TelemetryClient insightsClient;
        private readonly TelemetriaOptions telemetriaOptions;

        public ServicoTelemetria(TelemetryClient insightsClient, TelemetriaOptions telemetriaOptions)
        {
            this.insightsClient = insightsClient;
            this.telemetriaOptions = telemetriaOptions ?? throw new ArgumentNullException(nameof(telemetriaOptions));
        }

        public void Registrar(Action acao, string acaoNome, string telemetriaNome, string telemetriaValor)
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                transactionElk.CaptureSpan(telemetriaNome, "db", (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    acao();
                }, "postgresql", acaoNome);
            }
            else
            {
                acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();
                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }
        }

        public async Task RegistrarAsync(Func<Task> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                await transactionElk.CaptureSpan(telemetriaNome, "db", async (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("parametros", parametros);

                    await acao();
                }, "postgresql", acaoNome);
            }
            else
            {
                await acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();
                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }
        }

        public async Task RegistrarAsync(Func<Task> acao, string acaoNome, string telemetriaNome)
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                await transactionElk.CaptureSpan(telemetriaNome, "db", async (span) =>
                {
                    await acao();
                }, "postgresql", acaoNome);
            }
            else
            {
                await acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();
                insightsClient?.TrackDependency(acaoNome, telemetriaNome, string.Empty, inicioOperacao, temporizador.Elapsed, true);
            }
        }

        public dynamic RegistrarComRetorno<T>(Func<object> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            dynamic result = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                transactionElk.CaptureSpan(telemetriaNome, "db", (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("parametros", parametros);

                    result = acao();
                }, "postgresql", acaoNome);
            }
            else
            {
                result = acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();

                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }

            return result;
        }

        public async Task<dynamic> RegistrarComRetornoAsync<T>(Func<Task<object>> acao, string acaoNome, string telemetriaNome, string telemetriaValor, string parametros = "")
        {
            DateTime inicioOperacao = default;
            Stopwatch temporizador = default;

            dynamic result = default;

            if (telemetriaOptions.ApplicationInsights)
            {
                inicioOperacao = DateTime.UtcNow;
                temporizador = Stopwatch.StartNew();
            }

            if (telemetriaOptions.Apm)
            {
                var transactionElk = Agent.Tracer.CurrentTransaction;

                await transactionElk.CaptureSpan(telemetriaNome, "db", async (span) =>
                {
                    span.SetLabel(telemetriaNome, telemetriaValor);
                    span.SetLabel("parametros", parametros);

                    result = await acao();
                }, "postgresql", acaoNome);
            }
            else
            {
                result = await acao();
            }

            if (telemetriaOptions.ApplicationInsights)
            {
                temporizador.Stop();

                insightsClient?.TrackDependency(acaoNome, telemetriaNome, telemetriaValor, inicioOperacao, temporizador.Elapsed, true);
            }

            return result;
        }
    }
}
