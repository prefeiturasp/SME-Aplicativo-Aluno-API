using Microsoft.Extensions.DependencyInjection;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Infra.Persistencia.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Worker
{
    public static class InjecaoDependenciaExtension
    {
        #region Casos de Uso
        public static IServiceCollection AdicionarCasosDeUso(this IServiceCollection services)
        {
            return services
                .AddTransient<IAtualizarDadosUsuarioProdamUseCase, AtualizarDadosUsuarioProdamUseCase>();
        }
        #endregion

        #region Repositorios
        public static IServiceCollection AdicionarRepositorios(this IServiceCollection services)
        {
            return services
                .AddTransient<IUsuarioRepository, UsuarioRepository>()
                .AddTransient<IResponsavelEOLRepositorio, ResponsavelEOLRepositorio>();
        }
        #endregion
    }
}
