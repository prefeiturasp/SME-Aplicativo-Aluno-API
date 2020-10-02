using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
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
                ;
        }
        #endregion

        #region Workers
        public static IServiceCollection AdicionarWorkerCasosDeUso(this IServiceCollection services)
        {
            return services
                .AddSingleton<IHostedService, TransferirEventoSgpWorker>()
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
            ;
        }
        #endregion
    }
}
