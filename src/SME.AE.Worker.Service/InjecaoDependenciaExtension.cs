using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Infra.Persistencia.Cache;
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
                ;
        }
        #endregion

        #region Workers
        public static IServiceCollection AdicionarWorkerCasosDeUso(this IServiceCollection services)
        {
            return services
                //.AddSingleton<IHostedService, TransferirEventoSgpWorker>()
                //.AddSingleton<IHostedService, ConsolidarAdesaoEOLWorker>()
                //.AddSingleton<IHostedService, TransferirFrequenciaSgpWorker>()
                .AddSingleton<IHostedService, TransferirNotaSgpWorker>()
                ;
        }
        #endregion

        #region Repositorios
        public static IServiceCollection AdicionarRepositorios(this IServiceCollection services)
        {
            return services
                .AddTransient<ICacheRepositorio, CacheRepositorio>()
                .AddTransient<IConnectionMultiplexerAe, ConnectionMultiplexerAe>()

                .AddTransient<IParametrosEscolaAquiRepositorio, ParametroEscolaAquiRepositorio>()

                .AddTransient<IEventoRepositorio, EventoRepositorio>()
                .AddTransient<IEventoSgpRepositorio, EventoSgpRepositorio>()

                .AddTransient<IResponsavelEOLRepositorio, ResponsavelEOLRepositorio>()
                .AddTransient<IDashboardAdesaoRepositorio, DashboardAdesaoRepositorio>()
                .AddTransient<IWorkerProcessoAtualizacaoRepositorio, WorkerProcessoAtualizacaoRepositorio>()
                .AddTransient<IUsuarioRepository, UsuarioRepository>()

                .AddTransient<IFrequenciaAlunoRepositorio, FrequenciaAlunoRepositorio>()
                .AddTransient<IFrequenciaAlunoSgpRepositorio, FrequenciaAlunoSgpRepositorio>()
            ;
        }
        #endregion
    }
}
