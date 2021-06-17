using Npgsql;
using SME.AE.Comum;

namespace SME.AE.Infra.Persistencia.Repositorios.Base
{
    // TO DO: Colocar os demais reposiórios para herdar da classe base
    public abstract class RepositorioAppBase
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public RepositorioAppBase(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new System.ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        protected NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
    }
}