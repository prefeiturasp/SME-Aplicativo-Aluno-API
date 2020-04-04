using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comandos.Exemplo.ObterExemplo;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Middlewares;
using SME.AE.Aplicacao.Comum.Interfaces;

namespace SME.AE.Aplicacao
{
    public static class InjecaoDependencia 
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddFluentValidation(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());

            ////var config = new MapperConfiguration(cfg =>
            ////{
            ////    cfg.CreateMap<IAplicacaoContext, AplicacaoContext>
            ////})

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidacaoRequisicaoMiddleware<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExcecaoMiddleware<,>));

            //services.AddScoped(provider => provider.GetService<ObterExemploUseCase>());

            return services;
        }

        public static IServiceCollection AddFluentValidation(this IServiceCollection services, Assembly assembly)
        {
            var validatorType = typeof(IValidator<>);

            var validatorTypes = assembly
                .GetExportedTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == validatorType))
                .ToList();

            foreach (var validator in validatorTypes)
            {
                var requestType = validator.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == typeof(IValidator<>))
                    .Select(i => i.GetGenericArguments()[0])
                    .First();

                var validatorInterface = validatorType.MakeGenericType(requestType);
                services.AddTransient(validatorInterface, validator);
            }

            return services;
        }
    }
}
