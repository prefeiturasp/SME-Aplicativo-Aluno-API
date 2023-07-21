using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly.Extensions.Http;
using Polly;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.CasoDeUso.AgendadoWorkerService;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Repositorios;
using SME.AE.Worker.Service.CasoDeUsoWorker;
using System;
using System.Net.Http;
using System.Net;
using SME.AE.Comum.Utilitarios;
using Polly.Registry;

namespace SME.AE.Worker.Service
{
    public static class InjecaoDependenciaExtension
    {
        #region Casos de Uso
        public static IServiceCollection AdicionarCasosDeUso(this IServiceCollection services)
        {
            return services
                .AddTransient<ConsolidarAdesaoEOLCasoDeUso>()
                .AddTransient<ConsolidarLeituraNotificacaoCasoDeUso>()
                .AddTransient<EnviarNotificacaoDataFuturaCasoDeUso>()
                .AddTransient<RemoverConexaoIdleCasoDeUso>()
                .AddTransient<ICriarNotificacaoUseCase, CriarNotificacaoUseCase>();
        }
        #endregion
        #region Workers
        public static IServiceCollection AdicionarWorkerCasosDeUso(this IServiceCollection services)
        {
            return services
                .AddSingleton<IHostedService, ConsolidarAdesaoEOLWorker>()
                .AddSingleton<IHostedService, ConsolidarLeituraNotificacaoWorker>()
                .AddSingleton<IHostedService, EnviarNotificacaoDataFuturaWorker>()
                .AddSingleton<IHostedService, RemoverConexaoIdleWorker>()
                ;
        }
        #endregion

        #region Repositorios
        public static IServiceCollection AdicionarRepositorios(this IServiceCollection services)
        {
            return services
                .AddTransient<IParametrosEscolaAquiRepositorio, ParametroEscolaAquiRepositorio>()
                .AddTransient<IResponsavelEOLRepositorio, ResponsavelEOLRepositorio>()
                .AddTransient<IDashboardAdesaoRepositorio, DashboardAdesaoRepositorio>()
                .AddTransient<IWorkerProcessoAtualizacaoRepositorio, WorkerProcessoAtualizacaoRepositorio>()
                .AddTransient<IUsuarioRepository, UsuarioRepository>()
                .AddTransient<IConsolidarLeituraNotificacaoRepositorio, ConsolidarLeituraNotificacaoRepositorio>()
                .AddTransient<IConsolidarLeituraNotificacaoSgpRepositorio, ConsolidarLeituraNotificacaoSgpRepositorio>()
                .AddTransient<INotificacaoRepositorio, NotificacaoRepositorio>()
                .AddTransient<IDreSgpRepositorio, DreSgpRepositorio>()
                .AddTransient<IRemoverConexaoIdleRepository, RemoverConexaoIdleRepository>();
        }

        public static IServiceCollection AdicionarPoliticas(this IServiceCollection services)
        {
            IPolicyRegistry<string> registry = services.AddPolicyRegistry();

            Random jitterer = new Random();
            var policyFila = Policy.Handle<Exception>()
              .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      + TimeSpan.FromMilliseconds(jitterer.Next(0, 30)));

            registry.Add(PoliticaPolly.PublicaFila, policyFila);

            return services;
        }
        #endregion

        #region Clientes HTTP
        public static IServiceCollection AdicionarClientesHttp(this IServiceCollection services, ServicoProdamOptions servicoProdamOptions)
        {
            var policy = ObterPolicyBaseHttp();

            var basicAuth = $"{servicoProdamOptions.Usuario}:{servicoProdamOptions.Senha}".EncodeTo64();

            services.AddHttpClient(name: "servicoAtualizacaoCadastralProdam", c =>
            {
                c.BaseAddress = new Uri(servicoProdamOptions.Url);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("Authorization", $"Basic {basicAuth}");
            }).AddPolicyHandler(policy);

            return services;
        }

        static IAsyncPolicy<HttpResponseMessage> ObterPolicyBaseHttp()
        {
            return HttpPolicyExtensions
                 .HandleTransientHttpError()
                 .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                 .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                             retryAttempt)));
        }
        #endregion
    }
}
