using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SME.AE.Comum;
using System;
using System.Net;
using System.Net.Http;

namespace SME.AE.Api.Configuracoes
{
    public static class RegistraClientesHttp
    {
        public static void Registrar(IServiceCollection services, VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            var policy = ObterPolicyBaseHttp();

            services.AddHttpClient(name: "servicoAtualizacaoCadastral", c =>
            {
                c.BaseAddress = new Uri(variaveisGlobaisOptions.ApiPalavrasBloqueadas);
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            }).AddPolicyHandler(policy);

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
