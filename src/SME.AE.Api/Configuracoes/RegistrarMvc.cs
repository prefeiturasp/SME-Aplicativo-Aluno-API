using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Api.Filtros;
using SME.AE.Comum;

namespace SME.AE.Api.Configuracoes
{
    public static class RegistrarMvc
    {
        public static void Registrar(IServiceCollection services, VariaveisGlobaisOptions variaveisGlobais)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddMvc(options =>
            {
                //options.AllowValidatingTopLevelNodes = false;
                options.EnableEndpointRouting = true;
                options.Filters.Add(new ValidaDtoAttribute());
                options.Filters.Add(new FiltroExcecoesAttribute(variaveisGlobais));
            })
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}
