using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Worker.Service
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UseCaseWorkerAttribute : Attribute
    {
        public string CronParametroDB { get; set; }
        public string CronPadrao { get; set; }
    }

    public class UseCaseWorker<T> : IHostedService
    {
        private readonly IParametrosEscolaAquiRepositorio parametrosEscolaAqui;
        private readonly IServiceProvider serviceProvider;
        private readonly ILogger<UseCaseWorker<T>> logger;

        public UseCaseWorker(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            this.parametrosEscolaAqui = serviceProvider.GetRequiredService<IParametrosEscolaAquiRepositorio>() ?? throw new Exception($"Injeção de dependencia para o tipo IParametrosEscolaAquiRepositorio não registrado.");
            this.logger = serviceProvider.GetRequiredService<ILogger<UseCaseWorker<T>>>() ?? throw new Exception($"Injeção de dependencia para o tipo ILogger não registrado.");
        }

        private CrontabSchedule BuscaParametroCrontab()
        {
            var atributo = this.GetType().GetCustomAttribute<UseCaseWorkerAttribute>();
            if (atributo != null)
            {
                if (!string.IsNullOrWhiteSpace(atributo.CronParametroDB))
                {
                    if (parametrosEscolaAqui.TentaObterString(atributo.CronParametroDB, out var valorParametro))
                    {
                        if (!string.IsNullOrWhiteSpace(valorParametro))
                        {
                            return CrontabSchedule.Parse(valorParametro, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(atributo.CronPadrao))
                {
                    var cron = CrontabSchedule.Parse(atributo.CronPadrao, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                    parametrosEscolaAqui.Salvar(atributo.CronParametroDB, atributo.CronPadrao);
                    return cron;
                }
            }
            parametrosEscolaAqui.Salvar(atributo.CronParametroDB, "* * * * *");
            return CrontabSchedule.Parse("* * * * *");
        }

        private async Task ExecutaCasoDeUso()
        {
            SentrySdk.CaptureMessage($"Executando caso de uso {typeof(T).Name} => {DateTime.Now}");
            logger?.LogInformation($"Executando caso de uso {typeof(T).Name} => {DateTime.Now}");

            var servico = serviceProvider.GetService<T>() ?? throw new Exception($"Injeção de dependencia para o tipo {typeof(T).Name} não registrado.");
            var metodo =
                servico.GetType().GetMethod("ExecutarAsync")
                ?? servico.GetType().GetMethod("Executar")
                ?? throw new Exception($"Metodo Executar/ExecutarAsync não encontrado na classe {typeof(T).Name}.");

            if (metodo.ReturnType == typeof(Task))
            {
                await (Task)metodo.Invoke(servico, null);
            }
            else
            {
                metodo.Invoke(servico, null);
                await Task.CompletedTask;
            }
            return;
        }

        public Task StartAsync(CancellationToken cancelationTocken)
        {
            logger?.LogInformation("Hosted service starting");

            return Task.Factory.StartNew(async () =>
            {
                // loop until a cancalation is requested
                while (!cancelationTocken.IsCancellationRequested)
                {
                    logger?.LogInformation("Hosted service executing - {0}", DateTime.Now);
                    try
                    {
                        var crontab = BuscaParametroCrontab();
                        while (!cancelationTocken.IsCancellationRequested)
                        {
                            var proximaOcorrencia = crontab.GetNextOccurrence(DateTime.Now);
                            TimeSpan tempoAteProximaExec = proximaOcorrencia - DateTime.Now;

                            logger?.LogInformation($"Agendado caso de uso {typeof(T).Name} => {proximaOcorrencia}");
                            await Task.Delay(tempoAteProximaExec, cancelationTocken);

                            if (!cancelationTocken.IsCancellationRequested)
                            {
                                await Task.WhenAll(
                                    ExecutaCasoDeUso(),
                                    Task.Delay(10000)
                                );
                                GC.Collect();
                            }
                        }
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureMessage("*** Worker error: " + ex.Message);
                        SentrySdk.CaptureException(ex);
                        logger?.LogError(ex, "*** Worker error:");
                        throw ex;
                    }
                }
            }, cancelationTocken);
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger?.LogInformation("Hosted service stopping");
            return Task.CompletedTask;
        }
    }
}
