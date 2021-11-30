using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SME.AE.Aplicacao.HandlerExtensions;
using SME.AE.Comum;
using SME.AE.Comum.Utilitarios;
using System;
using System.Net;
using System.Net.Http;

namespace SME.AE.Api.Configuracoes
{
    public class RegistrarClientesHttp
    {
        public static void Registrar(IServiceCollection services, ServicoProdamOptions servicoProdamOptions, VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            var policy = ObterPolicyBaseHttp();

            var basicAuth = $"{servicoProdamOptions.Usuario}:{servicoProdamOptions.Senha}".EncodeTo64();

            services.AddTransient<AutenticacaoSgpDelegatingHandler>();

            services.AddHttpClient(name: "servicoAtualizacaoCadastralProdam", c =>
            {
                c.BaseAddress = new Uri(servicoProdamOptions.Url);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("Authorization", $"Basic {basicAuth}");
            }).AddPolicyHandler(policy);

            services.AddHttpClient(name: "servicoAtualizacaoCadastral", c =>
            {
                c.BaseAddress = new Uri(variaveisGlobaisOptions.ApiPalavrasBloqueadas);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(policy);

            services.AddHttpClient(name: "servicoApiSgp", c =>
            {
                c.BaseAddress = new Uri(variaveisGlobaisOptions.ApiSgp);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
                c.DefaultRequestHeaders.Add("x-sgp-api-key", variaveisGlobaisOptions.ChaveIntegracaoSgpApi);
            }).AddPolicyHandler(policy)
            .AddHttpMessageHandler<AutenticacaoSgpDelegatingHandler>();
        }

        static IAsyncPolicy<HttpResponseMessage> ObterPolicyBaseHttp()
        {
            return HttpPolicyExtensions
                 .HandleTransientHttpError()
                 .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                 .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                             retryAttempt)));
        }
    }
}
