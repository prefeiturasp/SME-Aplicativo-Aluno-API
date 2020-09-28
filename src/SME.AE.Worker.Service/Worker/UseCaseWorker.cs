using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NCrontab;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Worker.Service
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UseCaseWorkerAttribute: Attribute
    {
        public string ParametroEscolaAqui { get; set; }
        public string ParametroPadrao { get; set; }
    }

    public class UseCaseWorker<T> : BackgroundService
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
            if(atributo != null)
            {
                if (!string.IsNullOrWhiteSpace(atributo.ParametroEscolaAqui)) {
                    if (parametrosEscolaAqui.TentaObterString(atributo.ParametroEscolaAqui, out var valorParametro)) {
                        if (!string.IsNullOrWhiteSpace(valorParametro))
                        {
                            return CrontabSchedule.Parse(valorParametro, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                        }
                    } 
                }
                if (!string.IsNullOrWhiteSpace(atributo.ParametroPadrao))
                {
                    var cron = CrontabSchedule.Parse(atributo.ParametroPadrao, new CrontabSchedule.ParseOptions { IncludingSeconds = false });
                    parametrosEscolaAqui.Salvar(atributo.ParametroEscolaAqui, atributo.ParametroPadrao);
                    return cron;
                }
            }
            parametrosEscolaAqui.Salvar(atributo.ParametroEscolaAqui, "* * * * *");
            return CrontabSchedule.Parse("* * * * *"); 
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var crontab = BuscaParametroCrontab();
                while (!stoppingToken.IsCancellationRequested)
                {
                    var proximaOcorrencia = crontab.GetNextOccurrence(DateTime.Now);
                    TimeSpan tempoAteProximaExec = proximaOcorrencia - DateTime.Now;
                    await Task.Delay(tempoAteProximaExec, stoppingToken);

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await ExecutaCasoDeUso();
                    }
                }
            } catch (TaskCanceledException tce) { 
                // normal, ignora
            } catch (Exception ex) {
                // adicionar sentry
                logger.LogError(ex, "Worker error:");
                throw ex;
            }
        }

        private async Task ExecutaCasoDeUso()
        {
            var servico = serviceProvider.GetService<T>() ?? throw new Exception($"Injeção de dependencia para o tipo {typeof(T).Name} não registrado.");
            var metodo = 
                servico.GetType().GetMethod("ExecutarAsync") 
                ?? servico.GetType().GetMethod("Executar") 
                ?? throw new Exception($"Metodo Executar/ExecutarAsync não encontrado na classe {typeof(T).Name}.");

            if (metodo.ReturnType == typeof(Task))
            {
                await (Task)metodo.Invoke(servico, null);
            } else
            {
                metodo.Invoke(servico, null);
                await Task.CompletedTask;
            }
            return;
        }
    }
}
