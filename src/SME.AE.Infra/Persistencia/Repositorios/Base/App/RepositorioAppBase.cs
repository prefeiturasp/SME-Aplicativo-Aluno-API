using Npgsql;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Infra.Persistencia.Repositorios.Base
{
    // TO DO: Colocar os demais reposiórios para herdar da classe base
    public abstract class RepositorioAppBase
    {
        protected NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);
    }
}