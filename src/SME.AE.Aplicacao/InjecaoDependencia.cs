using System.Linq;
using System.Reflection;
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.ObterPorCpf;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Middlewares;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using static SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario.AutenticarUsuarioCommand;
using SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida;

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
            
            // Notificacao
            services.AddScoped(provider => provider.GetService<CriarNotificacaoUseCase>());
            services.AddScoped(provider => provider.GetService<AtualizarNotificacaoUseCase>());
            services.AddScoped(provider => provider.GetService<ObterNotificacaoPorGrupoUseCase>());
            services.AddScoped(provider => provider.GetService<RemoverNotificacaoEmLoteUseCase>());
            services.AddScoped(provider => provider.GetService<ObterDoUsuarioLogadoUseCase>());

            // Mensagem
            services.AddScoped(provider => provider.GetService<MarcarMensagemLidaUseCase>());
        }

        private static void AddComandos(IServiceCollection services)
        {
            // Usuario
            services.AddMediatR(typeof(ObterUsuarioPorCpfCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterUsuarioPorCpfCommandHandler).GetTypeInfo().Assembly);

            // Token
            services.AddMediatR(typeof(CriarTokenCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(CriarTokenCommandHandler).GetTypeInfo().Assembly);
            
            // Autenticacao
            services.AddMediatR(typeof(AutenticarUsuarioCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(AutenticarUsuarioCommandHandler).GetTypeInfo().Assembly);
            
            // Notificacao
            services.AddMediatR(typeof(CriarNotificacaoCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(CriarNotificacaoCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(AtualizarNotificacaoCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(AtualizarNotificacaoCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterNotificacaoPorGrupoCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterNotificacaoPorGrupoCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(RemoverNotificacaoCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(RemoverNotificacaoCommandHandler).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterGrupoNotificacaoPorResponsavelCommand).GetTypeInfo().Assembly);
            services.AddMediatR(typeof(ObterGrupoNotificacaoPorResponsavelCommandHandler).GetTypeInfo().Assembly);

            //Mensagem
            services.AddMediatR(typeof(AtualizarNotificacaoCommand).GetTypeInfo().Assembly);
        }
    }
}
