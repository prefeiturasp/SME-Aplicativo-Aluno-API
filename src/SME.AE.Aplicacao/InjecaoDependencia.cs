using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.AE.Aplicacao.CasoDeUso.TesteArquitetura;
using SME.AE.Aplicacao.Comum.Middlewares;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.CasoDeUso.Usuario;

namespace SME.AE.Aplicacao
{
    public static class InjecaoDependencia
    {
        public static void AdicionarMediatr(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("SME.AE.Aplicacao");
            services.AddMediatR(assembly);
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
          
            AdicionarMediatr(services);
            AddFiltros(services);
            AddServices(services);
            AddCasosDeUso(services);

            return services;
        }

        private static void AddServices(IServiceCollection services) { }

        private static void AddFiltros(IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidacaoRequisicaoMiddleware<,>));
        }

        private static void AddCasosDeUso(IServiceCollection services)
        {
            services.TryAddScoped<ITesteArquiteturaUseCase, TesteArquiteturaUseCase>();

            //Usuario
            services.TryAddScoped(typeof(ICriarUsuarioPrimeiroAcessoUseCase), typeof(CriarUsuarioPrimeiroAcessoUseCase));
            services.TryAddScoped(typeof(IAlterarEmailCelularUseCase), typeof(AlterarEmailCelularUseCase));
        }
    }
}
