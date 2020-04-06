using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.ObterPorCpf;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Middlewares;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using static SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario.AutenticarUsuarioCommand;

namespace SME.AE.Aplicacao
{
    public static class InjecaoDependencia
    {
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

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddFluentValidation(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            
            AddFiltros(services);
            AddServices(services);
            AddCasosDeUso(services);
            AddComandos(services);
            
            return services;
        }

        private static void AddServices(IServiceCollection services) {}

        private static void AddFiltros(IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidacaoRequisicaoMiddleware<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExcecaoMiddleware<,>));
        }

        private static void AddCasosDeUso(IServiceCollection services)
        {
            services.AddScoped(provider => provider.GetService<AutenticarUsuarioUseCase>());
        }

        private static void AddComandos(IServiceCollection services)
        {
            services.AddMediatR(typeof(ObterUsuarioPorCpfCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterUsuarioPorCpfCommandHandler).GetTypeInfo().Assembly);

            services.AddMediatR(typeof(CriarTokenCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(CriarTokenCommandHandler).GetTypeInfo().Assembly);
            
            services.AddMediatR(typeof(AutenticarUsuarioCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(AutenticarUsuarioCommandHandler).GetTypeInfo().Assembly);
        }
    }
}
