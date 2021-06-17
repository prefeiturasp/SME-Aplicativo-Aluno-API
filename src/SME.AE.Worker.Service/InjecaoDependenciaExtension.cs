using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.CasoDeUso.AgendadoWorkerService;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Infra.Persistencia.Repositorios;
using SME.AE.Worker.Service.CasoDeUsoWorker;

namespace SME.AE.Worker.Service
{
    public static class InjecaoDependenciaExtension
    {
        #region Casos de Uso
        public static IServiceCollection AdicionarCasosDeUso(this IServiceCollection services)
        {
            return services
                .AddTransient<TranferirEventoSgpCasoDeUso>()
                .AddTransient<ConsolidarAdesaoEOLCasoDeUso>()
                .AddTransient<TransferirFrequenciaSgpCasoDeUso>()
                .AddTransient<TransferirNotaSgpCasoDeUso>()
                .AddTransient<ConsolidarLeituraNotificacaoCasoDeUso>()
                .AddTransient<EnviarNotificacaoDataFuturaCasoDeUso>()
                .AddTransient<RemoverConexaoIdleCasoDeUso>()
                .AddTransient<ICriarNotificacaoUseCase, CriarNotificacaoUseCase>();
        }
        #endregion
        #region Workers
        public static IServiceCollection AdicionarWorkerCasosDeUso(this IServiceCollection services)
        {
            return services
                //.AddSingleton<IHostedService, TransferirEventoSgpWorker>()
                .AddSingleton<IHostedService, ConsolidarAdesaoEOLWorker>()
                .AddSingleton<IHostedService, TransferirFrequenciaSgpWorker>()
                .AddSingleton<IHostedService, TransferirNotaSgpWorker>()
                .AddSingleton<IHostedService, ConsolidarLeituraNotificacaoWorker>()
                .AddSingleton<IHostedService, EnviarNotificacaoDataFuturaWorker>()
                .AddSingleton<IHostedService, RemoverConexaoIdleWorker>()
                ;
        }
        #endregion

        #region Repositorios
        public static IServiceCollection AdicionarRepositorios(this IServiceCollection services)
        {
            return services
                .AddTransient<IParametrosEscolaAquiRepositorio, ParametroEscolaAquiRepositorio>()
                .AddTransient<IEventoRepositorio, EventoRepositorio>()
                .AddTransient<IEventoSgpRepositorio, EventoSgpRepositorio>()
                .AddTransient<IResponsavelEOLRepositorio, ResponsavelEOLRepositorio>()
                .AddTransient<IDashboardAdesaoRepositorio, DashboardAdesaoRepositorio>()
                .AddTransient<IWorkerProcessoAtualizacaoRepositorio, WorkerProcessoAtualizacaoRepositorio>()
                .AddTransient<IUsuarioRepository, UsuarioRepository>()
                .AddTransient<IFrequenciaAlunoRepositorio, FrequenciaAlunoRepositorio>()
                .AddTransient<IFrequenciaAlunoSgpRepositorio, FrequenciaAlunoSgpRepositorio>()
                .AddTransient<INotaAlunoRepositorio, NotaAlunoRepositorio>()
                .AddTransient<INotaAlunoSgpRepositorio, NotaAlunoSgpRepositorio>()
                .AddTransient<IConsolidarLeituraNotificacaoRepositorio, ConsolidarLeituraNotificacaoRepositorio>()
                .AddTransient<IConsolidarLeituraNotificacaoSgpRepositorio, ConsolidarLeituraNotificacaoSgpRepositorio>()
                .AddTransient<INotificacaoRepository, NotificacaoRepository>()
                .AddTransient<IDreSgpRepositorio, DreSgpRepositorio>()
                .AddTransient<IRemoverConexaoIdleRepository, RemoverConexaoIdleRepository>();
        }
        #endregion
    }
}
