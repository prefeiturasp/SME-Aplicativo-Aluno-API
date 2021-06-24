using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Mapeamentos;
using System;

namespace SME.AE.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            AdicionarMediatr(services);
            ConfiguraVariaveisAmbiente(services);
            ConfiguraSentry();
            var servicoProdam = new ServicoProdamOptions();
            Configuration.GetSection(nameof(ServicoProdamOptions)).Bind(servicoProdam, c => c.BindNonPublicProperties = true);

            services.AddSingleton(servicoProdam);
            
            RegistrarMapeamentos.Registrar();

            services
                .AdicionarRepositorios()
                .AdicionarIdentity()
                .AdicionarServicos()
                .AdicionarCasosDeUso()
                .AdicionarPoliticas()
                .AdicionarClientesHttp(servicoProdam)
                .AddMemoryCache()
                .AddApplicationInsightsTelemetry()
                .AddHostedService<WorkerRabbitMQ>();

           

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SME.AE.Worker", Version = "v1" });
            });
        }

        private void ConfiguraSentry()
        {
            Sentry.SentrySdk.Init(Configuration.GetSection("Sentry:DSN").Value);            
        }
        
        private void ConfiguraVariaveisAmbiente(IServiceCollection services)
        {
            var variaveisGlobais = new VariaveisGlobaisOptions();
            Configuration.GetSection(nameof(VariaveisGlobaisOptions)).Bind(variaveisGlobais, c => c.BindNonPublicProperties = true);

            services.AddSingleton(variaveisGlobais);

            var configuracaoRabbitOptions = new ConfiguracaoRabbitOptions();
            Configuration.GetSection(nameof(ConfiguracaoRabbitOptions)).Bind(configuracaoRabbitOptions, c => c.BindNonPublicProperties = true);

            var rabbitConn = new ConnectionFactory
            {
                HostName = configuracaoRabbitOptions.HostName,
                UserName = configuracaoRabbitOptions.UserName,
                Password = configuracaoRabbitOptions.Password,
                VirtualHost = configuracaoRabbitOptions.VirtualHost
            };

            services.AddSingleton(rabbitConn);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SME.AE.Worker v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AdicionarMediatr(IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("SME.AE.Aplicacao");
            services.AddMediatR(assembly);
        }
    }
}
