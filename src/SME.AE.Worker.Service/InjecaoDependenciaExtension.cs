using Microsoft.Extensions.DependencyInjection;
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
                .AddHostedService<TransferirEventoSgpWorker>()
                ;
        }
        #endregion

        #region Repositorios
        public static IServiceCollection AdicionarRepositorios(this IServiceCollection services)
        {
            return services
                .AddTransient<IParametrosEscolaAquiRepositorio, ParametroEscolaAquiRepositorio>()
                ;
        }
        #endregion
    }
}
