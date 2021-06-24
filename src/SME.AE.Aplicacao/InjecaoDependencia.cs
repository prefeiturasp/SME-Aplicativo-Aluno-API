using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;
using Polly.Registry;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.CasoDeUso.Aluno;
using SME.AE.Aplicacao.CasoDeUso.Frequencia;
using SME.AE.Aplicacao.CasoDeUso.Notificacao;
using SME.AE.Aplicacao.CasoDeUso.TermosDeUso;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.CasoDeUso.UsuarioNotificacaoMensagemLida;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Frequencia;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.UltimaAtualizacaoWorker;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Middlewares;
using SME.AE.Aplicacao.Servicos;
using SME.AE.Comum;
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
            services.AddPolicies();
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
        public static void AddPolicies(this IServiceCollection services)
        {
            IPolicyRegistry<string> registry = services.AddPolicyRegistry();

            Random jitterer = new Random();
            var policyFila = Policy.Handle<Exception>()
              .WaitAndRetryAsync(3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                      + TimeSpan.FromMilliseconds(jitterer.Next(0, 30)));

            registry.Add(PoliticaPolly.PublicaFila, policyFila);
        }

        private static void AddCasosDeUso(this IServiceCollection services)
        {
            //Usuario
            services.TryAddScoped<IMarcarMensagemLidaUseCase, MarcarMensagemLidaUseCase>();
            services.TryAddScoped<IPrimeiroAcessoUseCase, PrimeiroAcessoUseCase>();
            services.TryAddScoped<ICriarNotificacaoUseCase, CriarNotificacaoUseCase>();
            services.TryAddScoped<IAtualizarNotificacaoUseCase, AtualizarNotificacaoUseCase>();
            services.TryAddScoped<IRemoverNotificacaoEmLoteUseCase, RemoverNotificacaoEmLoteUseCase>();
            services.TryAddScoped<IRemoveNotificacaoPorIdUseCase, RemoveNotificacaoPorIdUseCase>();
            services.TryAddScoped<IObterNotificacaoDoUsuarioLogadoUseCase, ObterNotificacaoDoUsuarioLogadoUseCase>();
            services.TryAddScoped<IAutenticarUsuarioUseCase, AutenticarUsuarioUseCase>();
            services.TryAddScoped<IDadosDoAlunoUseCase, DadosDoAlunoUseCase>();
            services.TryAddScoped<IAlterarSenhaUseCase, AlterarSenhaUseCase>();
            services.TryAddScoped<ISolicitarRedifinicaoSenhaUseCase, SolicitarRedifinicaoSenhaUseCase>();
            services.TryAddScoped<IValidarTokenUseCase, ValidarTokenUseCase>();
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
            services.TryAddScoped<IObterTotalUsuariosComAcessoIncompletoUseCase, ObterTotalUsuariosComAcessoIncompletoUseCase>();
            services.TryAddScoped<IObterTotalUsuariosValidosUseCase, ObterTotalUsuariosValidosUseCase>();
            services.TryAddScoped<IObterTotaisAdesaoUseCase, ObterTotaisAdesaoUseCase>();
            services.TryAddScoped<IObterTotaisAdesaoAgrupadosPorDreUseCase, ObterTotaisAdesaoAgrupadosPorDreUseCase>();
            services.TryAddScoped<IObterUltimaAtualizacaoPorProcessoUseCase, ObterUltimaAtualizacaoPorProcessoUseCase>();
            services.TryAddScoped<IAtualizarDadosUsuarioUseCase, AtualizarDadosUsuarioUseCase>();
            services.TryAddScoped<IObterFrequenciaAlunoPorComponenteCurricularUseCase, ObterFrequenciaAlunoPorComponenteCurricularUseCase>();
            services.TryAddScoped<IObterFrequenciaAlunoUseCase, ObterFrequenciaAlunoUseCase>();
            services.TryAddScoped<IObterNotasAlunoUseCase, ObterNotasAlunoUseCase>();
            services.TryAddScoped<IValidarUsuarioEhResponsavelDeAlunoUseCase, ValidarUsuarioEhResponsavelDeAlunoUseCase>();
            services.TryAddScoped<IObterDadosDeLeituraComunicadosUseCase, ObterDadosDeLeituraComunicadosUseCase>();
            services.TryAddScoped<IObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase, ObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase>();
            services.TryAddScoped<IObterDadosUnidadeEscolarUseCase, ObterDadosUnidadeEscolarUseCase>();
            services.TryAddScoped<IObterDadosDeLeituraModalidadeUseCase, ObterDadosDeLeituraModalidadeUseCase>();
            services.TryAddScoped<IObterDadosDeLeituraTurmaUseCase, ObterDadosDeLeituraTurmaUseCase>();
            services.TryAddScoped<IObterDadosDeLeituraAlunosUseCase, ObterDadosDeLeituraAlunosUseCase>();
            services.TryAddScoped<IObterStatusDeLeituraNotificacaoUseCase, ObterStatusDeLeituraNotificacaoUseCase>();
            services.TryAddScoped<IAtualizarDadosUsuarioProdamUseCase, AtualizarDadosUsuarioProdamUseCase>();
            services.TryAddScoped<IAtualizarDadosUsuarioEolUseCase, AtualizarDadosUsuarioEolUseCase>();
            services.TryAddScoped<IObterDadosUsuarioPorCpfUseCase, ObterDadosUsuarioPorCpfUseCase>();            
        }
    }
}