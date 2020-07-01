using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Aplicacao.CasoDeUso.TesteArquitetura;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Middlewares;
using System;

namespace SME.AE.Aplicacao
{
    public static class InjecaoDependencia
    {
        private static void AdicionarMediatr(this IServiceCollection services)
        {
            var assembly = AppDomain.CurrentDomain.Load("SME.AE.Aplicacao");
            services.AddMediatR(assembly);
        }

        public static void AddApplication(this IServiceCollection services)
        {
            services.AdicionarAutoMapper();
            services.AdicionarMediatr();
            services.AddFiltros();
            services.AddServices();
            services.AddCasosDeUso();
        }

        private static void AdicionarAutoMapper(this IServiceCollection services)
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddMaps(AppDomain.CurrentDomain.Load("SME.AE.Aplicacao"));
            });

            services.AddSingleton(configuration.CreateMapper());
        }

        private static void AddServices(this IServiceCollection services) { }

        private static void AddFiltros(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidacaoRequisicaoMiddleware<,>));
        }

        private static void AddCasosDeUso(this IServiceCollection services)
        {
            services.TryAddScoped<ITesteArquiteturaUseCase, TesteArquiteturaUseCase>();

            //Usuario
            services.TryAddScoped(typeof(IMarcarMensagemLidaUseCase), typeof(MarcarMensagemLidaUseCase));
            services.TryAddScoped(typeof(ICriarUsuarioPrimeiroAcessoUseCase), typeof(PrimeiroAcessoUseCase));
            services.TryAddScoped(typeof(IAlterarEmailCelularUseCase), typeof(AlterarEmailCelularUseCase));
            services.TryAddScoped(typeof(ICriarNotificacaoUseCase), typeof(CriarNotificacaoUseCase));
            services.TryAddScoped(typeof(IAtualizarNotificacaoUseCase), typeof(AtualizarNotificacaoUseCase));
            services.TryAddScoped(typeof(IRemoverNotificacaoEmLoteUseCase), typeof(RemoverNotificacaoEmLoteUseCase));
            services.TryAddScoped(typeof(IRemoveNotificacaoPorIdUseCase), typeof(RemoveNotificacaoPorIdUseCase));
            services.TryAddScoped(typeof(IObterNotificacaoDoUsuarioLogadoUseCase), typeof(ObterNotificacaoDoUsuarioLogadoUseCase));
            services.TryAddScoped(typeof(IAutenticarUsuarioUseCase), typeof(AutenticarUsuarioUseCase));
            services.TryAddScoped(typeof(IDadosDoAlunoUseCase), typeof(DadosDoAlunoUseCase));
            
        }
    }
}
