using Npgsql;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Infra.Persistencia.Repositorios.Base
{
    public abstract class RepositorioAppBase
    {
        protected NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);
    }
}