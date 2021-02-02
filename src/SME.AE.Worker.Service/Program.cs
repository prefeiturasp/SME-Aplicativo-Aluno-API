using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Infra.Persistencia.Mapeamentos;
using System;

namespace SME.AE.Worker.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RegistrarMapeamentos.Registrar();
            CreateHostBuilder(args).Build().Run();
        }

        private static void AdicionarAutoMapper(IServiceCollection services)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.Load("SME.AE.Aplicacao"));
            });

            services.AddSingleton(configuration.CreateMapper());
        }
        private static void AdicionarMediatr(IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("SME.AE.Aplicacao");
            services.AddMediatR(assembly);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    AdicionarAutoMapper(services);
                    AdicionarMediatr(services);
                    services
                        .AdicionarRepositorios()
                        .AdicionarCasosDeUso()
                        .AdicionarWorkerCasosDeUso();
                }).ConfigureLogging((context, logging) =>
                    {
                        logging.AddConfiguration(context.Configuration);
                        logging.AddSentry(option => { option.Dsn = VariaveisAmbiente.SentryDsn; });
                    });
    }
}
