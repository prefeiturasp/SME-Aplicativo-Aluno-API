﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using Polly.Registry;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.HandlerExtensions;
using SME.AE.Aplicacao.Servicos;
using SME.AE.Comum;
using SME.AE.Comum.Utilitarios;
using SME.AE.Infra;
using SME.AE.Infra.Autenticacao;
using SME.AE.Infra.Persistencia;
using SME.AE.Infra.Persistencia.Repositorios;
using SME.AE.Infra.Persistencia.Repositorios.Externos;
using SME.AE.Infra.Persistencia.Repositorios.Externos.CoreSSO;
using System;
using System.Net;
using System.Net.Http;

namespace SME.AE.Worker
{
    public static class InjecaoDependenciaExtension
    {
        #region Casos de Uso
        public static IServiceCollection AdicionarCasosDeUso(this IServiceCollection services)
        {
            return services
                .AddTransient<IAtualizarDadosUsuarioProdamUseCase, AtualizarDadosUsuarioProdamUseCase>()
                .AddTransient<IAtualizarDadosUsuarioEolUseCase, AtualizarDadosUsuarioEolUseCase>();
        }
        #endregion

        #region Servicos
        public static IServiceCollection AdicionarServicos(this IServiceCollection services)
        {
            return services
                .AddTransient<IServicoLog, ServicoLog>()
                .AddTransient<IAutenticacaoService, AutenticacaoService>();
        }
        #endregion

        #region Repositorios
        public static IServiceCollection AdicionarRepositorios(this IServiceCollection services)
        {
            services.AddTransient(typeof(IUsuarioRepository), typeof(UsuarioRepository));
            services.AddTransient(typeof(INotificacaoRepositorio), typeof(NotificacaoRepositorio));
            services.AddTransient(typeof(IUsuarioNotificacaoRepositorio), typeof(UsuarioNotificacaoRepositorio));
            services.AddTransient(typeof(IUsuarioCoreSSORepositorio), typeof(UsuarioCoreSSORepositorio));
            services.AddTransient(typeof(IUsuarioGrupoRepositorio), typeof(UsuarioGrupoRepositorio));
            services.AddTransient(typeof(IUsuarioSenhaHistoricoCoreSSORepositorio), typeof(UsuarioSenhaHistoricoCoreSSORepositorio));
            services.AddTransient(typeof(IConfiguracaoEmailRepositorio), typeof(ConfiguracaoEmailRepositorio));
            services.AddTransient(typeof(ITermosDeUsoRepositorio), typeof(TermosDeUsoRepositorio));
            services.AddTransient(typeof(IAceiteTermosDeUsoRepositorio), typeof(AceiteTermosDeUsoRepositorio));
            services.AddTransient(typeof(IAdesaoRepositorio), typeof(AdesaoRepositorio));
            services.AddTransient(typeof(IParametrosEscolaAquiRepositorio), (typeof(ParametroEscolaAquiRepositorio)));
            services.AddTransient(typeof(IWorkerProcessoAtualizacaoRepositorio), typeof(WorkerProcessoAtualizacaoRepositorio));
            services.AddTransient(typeof(INotaAlunoCorRepositorio), typeof(NotaAlunoCorRepositorio));
            services.AddTransient(typeof(IDadosLeituraRepositorio), typeof(DadosLeituraRepositorio));
            services.AddTransient(typeof(IRemoverConexaoIdleRepository), typeof(RemoverConexaoIdleRepository));
            services.AddTransient(typeof(ICacheRepositorio), typeof(CacheRepositorio));
            services.AddTransient(typeof(IServicoLog), typeof(ServicoLog));
            services.AddTransient(typeof(IOutroServicoRepositorio), typeof(OutroServicoRepositorio));

            return services;
        }
        #endregion

        #region Identity
        public static IServiceCollection AdicionarIdentity(this IServiceCollection services)
        {
            services.AddDefaultIdentity<UsuarioAplicacao>().AddEntityFrameworkStores<AplicacaoContext>();
            services.AddIdentityServer().AddApiAuthorization<UsuarioAplicacao, AplicacaoContext>();
            services.AddScoped<IAutenticacaoService, AutenticacaoService>();
            services.AddAuthentication().AddIdentityServerJwt();

            return services;
        }
        #endregion

        #region Repositorios
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
        public static IServiceCollection AdicionarClientesHttp(this IServiceCollection services, ServicoProdamOptions servicoProdamOptions, VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            var policy = ObterPolicyBaseHttp();

            var basicAuth = $"{servicoProdamOptions.Usuario}:{servicoProdamOptions.Senha}".EncodeTo64();

            services.AddHttpClient(name: "servicoAtualizacaoCadastralProdam", c =>
             {
                 c.BaseAddress = new Uri(servicoProdamOptions.Url);
                 c.DefaultRequestHeaders.Add("Accept", "application/json");
                 c.DefaultRequestHeaders.Add("Authorization", $"Basic {basicAuth}");
             }).AddPolicyHandler(policy);

            services.AddHttpClient(name: "servicoApiSgpChave", c =>
            {
                c.BaseAddress = new Uri(variaveisGlobaisOptions.ApiSgp);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-sgp-api-key", variaveisGlobaisOptions.ChaveIntegracaoSgpApi);
            }).AddPolicyHandler(policy);

            services.AddHttpClient(name: "servicoApiEolChave", c =>
            {
                c.BaseAddress = new Uri(variaveisGlobaisOptions.ApiEol);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-api-eol-key", variaveisGlobaisOptions.ChaveIntegracaoEolApi);
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
