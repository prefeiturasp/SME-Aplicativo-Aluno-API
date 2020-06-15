﻿using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Api.Filtros;

namespace SME.AE.Api.Configuracoes
{
    public static class RegistrarMvc
    {
        public static void Registrar(IServiceCollection services, IConfiguration configuration)
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
                options.Filters.Add(new FiltroExcecoesAttribute(configuration));
            })
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }
    }
}
