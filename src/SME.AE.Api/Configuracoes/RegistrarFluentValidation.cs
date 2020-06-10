using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SME.AE.Api
{
    public static class RegistrarFluentValidation
    {
        public static void AdicionarValidadoresFluentValidation(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("SME.AE.Aplicacao");

            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}