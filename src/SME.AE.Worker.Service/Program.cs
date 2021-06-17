using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Mapeamentos;
using System;
using System.Reflection;

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
                .ConfigureAppConfiguration( a => a.AddUserSecrets(Assembly.GetExecutingAssembly()))
                .ConfigureServices((hostContext, services) =>
                {
                    
                    AdicionarAutoMapper(services);
                    AdicionarMediatr(services);
                    services
                        .AdicionarRepositorios()
                        .AdicionarCasosDeUso()
                        .AdicionarWorkerCasosDeUso();

                    var variaveisGlobais = new VariaveisGlobaisOptions();
                    hostContext.Configuration.GetSection(nameof(VariaveisGlobaisOptions)).Bind(variaveisGlobais, c => c.BindNonPublicProperties = true);

                    services.AddSingleton(variaveisGlobais);


                }).ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration);
                    logging.AddSentry();
                });
    }
}
