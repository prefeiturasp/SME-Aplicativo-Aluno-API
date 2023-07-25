using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Servicos;
using RabbitMQ.Client;
using SME.AE.Comum;
using SME.AE.Dominio.Options;
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

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var servicoProdam = new ServicoProdamOptions();
            config.GetSection(nameof(ServicoProdamOptions)).Bind(servicoProdam, c => c.BindNonPublicProperties = true);

            var variaveisGlobais = new VariaveisGlobaisOptions();
            config.GetSection(nameof(VariaveisGlobaisOptions)).Bind(variaveisGlobais, c => c.BindNonPublicProperties = true);

            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(a => a.AddUserSecrets(Assembly.GetExecutingAssembly()))
                .ConfigureServices((hostContext, services) =>
                {

                    AdicionarAutoMapper(services);
                    AdicionarMediatr(services);

                    services
                        .AddApplicationInsightsTelemetry(hostContext.Configuration)
                        .AdicionarRepositorios()
                        .AdicionarCasosDeUso()
                        .AdicionarWorkerCasosDeUso()
                        .AdicionarPoliticas()
                        .AdicionarClientesHttp(servicoProdam, variaveisGlobais);

                    var variaveisGlobais = new VariaveisGlobaisOptions();
                    hostContext.Configuration.GetSection(nameof(VariaveisGlobaisOptions)).Bind(variaveisGlobais, c => c.BindNonPublicProperties = true);

                    services.AddSingleton(variaveisGlobais);

                    var configuracaoRabbitOptions = new ConfiguracaoRabbitOptions();
                    config.GetSection(nameof(ConfiguracaoRabbitOptions)).Bind(configuracaoRabbitOptions, c => c.BindNonPublicProperties = true);

                    var rabbitConn = new ConnectionFactory
                    {
                        HostName = configuracaoRabbitOptions.HostName,
                        UserName = configuracaoRabbitOptions.UserName,
                        Password = configuracaoRabbitOptions.Password,
                        VirtualHost = configuracaoRabbitOptions.VirtualHost
                    };

                    services.AddSingleton(rabbitConn);

                    var telemetriaOptions = new TelemetriaOptions();
                    hostContext.Configuration.GetSection(TelemetriaOptions.Secao).Bind(telemetriaOptions, c => c.BindNonPublicProperties = true);

                    services.AddSingleton(telemetriaOptions);
                    services.AddSingleton<IServicoTelemetria, ServicoTelemetria>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration);
                    logging.AddSentry();
                });
        }
    }
}
