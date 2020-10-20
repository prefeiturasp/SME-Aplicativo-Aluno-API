using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Aplicacao.CasoDeUso.TermosDeUso;
using SME.AE.Aplicacao.CasoDeUso.TesteArquitetura;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Middlewares;
using SME.AE.Aplicacao.Servicos;
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

        private static void AddServices(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IEmailServico), typeof(EmailServico));
        }

        private static void AddFiltros(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidacaoRequisicaoMiddleware<,>));
        }

        private static void AddCasosDeUso(this IServiceCollection services)
        {
            services.TryAddScoped<ITesteArquiteturaUseCase, TesteArquiteturaUseCase>();

            //Usuario
            services.TryAddScoped(typeof(IMarcarMensagemLidaUseCase), typeof(MarcarMensagemLidaUseCase));
            services.TryAddScoped(typeof(IPrimeiroAcessoUseCase), typeof(PrimeiroAcessoUseCase));
            services.TryAddScoped(typeof(IAlterarEmailCelularUseCase), typeof(AlterarEmailCelularUseCase));
            services.TryAddScoped(typeof(ICriarNotificacaoUseCase), typeof(CriarNotificacaoUseCase));
            services.TryAddScoped(typeof(IAtualizarNotificacaoUseCase), typeof(AtualizarNotificacaoUseCase));
            services.TryAddScoped(typeof(IRemoverNotificacaoEmLoteUseCase), typeof(RemoverNotificacaoEmLoteUseCase));
            services.TryAddScoped(typeof(IRemoveNotificacaoPorIdUseCase), typeof(RemoveNotificacaoPorIdUseCase));
            services.TryAddScoped(typeof(IObterNotificacaoDoUsuarioLogadoUseCase), typeof(ObterNotificacaoDoUsuarioLogadoUseCase));
            services.TryAddScoped(typeof(IAutenticarUsuarioUseCase), typeof(AutenticarUsuarioUseCase));
            services.TryAddScoped(typeof(IDadosDoAlunoUseCase), typeof(DadosDoAlunoUseCase));
            services.TryAddScoped(typeof(IAlterarSenhaUseCase), typeof(AlterarSenhaUseCase));
            services.TryAddScoped(typeof(ISolicitarRedifinicaoSenhaUseCase), typeof(SolicitarRedifinicaoSenhaUseCase));
            services.TryAddScoped(typeof(IValidarTokenUseCase), typeof(ValidarTokenUseCase));
            services.TryAddScoped<IRedefinirSenhaUseCase, RedefinirSenhaUseCase>();
            services.TryAddScoped<IMensagensUsuarioLogadoAlunoUseCase, MensagensUsuarioLogadoAlunoUseCase>();
            services.TryAddScoped<IMensagenUsuarioLogadoAlunoIdUseCase, MensagenUsuarioLogadoAlunoIdUseCase>();
            services.TryAddScoped<IMarcarExcluidaMensagenUsuarioAlunoIdUseCase, MarcarExcluidaMensagenUsuarioAlunoIdUseCase>();
            services.TryAddScoped<ISolicitarReiniciarSenhaUseCase, SolicitarReiniciarSenhaUseCase>();
            services.TryAddScoped<IObterUsuarioUseCase, ObterUsuarioUseCase>();
            services.TryAddScoped<IObterTermosDeUsoUseCase, ObterTermosDeUsoUseCase>();
            services.TryAddScoped<IObterTermosDeUsoPorCpfUseCase, ObterTermosDeUsoPorCpfUseCase>();
            services.TryAddScoped<IRegistrarAceiteDosTermosDeUsoUseCase, RegistrarAceiteDosTermosDeUsoUseCase>();
            services.TryAddScoped<IObterEventosAlunoPorMesUseCase, ObterEventosAlunoPorMesUseCase>();
            services.TryAddScoped<IObterEventosAlunoPorDataUseCase, ObterEventosAlunoPorDataUseCase>();
            services.TryAddScoped<IObterTotalUsuariosComAcessoIncompletoUseCase, ObterTotalUsuariosComAcessoIncompletoUseCase>();
            services.TryAddScoped<IObterTotalUsuariosValidosUseCase, ObterTotalUsuariosValidosUseCase>();
        }
    }
}
