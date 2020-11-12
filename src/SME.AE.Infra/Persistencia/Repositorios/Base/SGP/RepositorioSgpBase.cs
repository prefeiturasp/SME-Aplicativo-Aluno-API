using Npgsql;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Infra.Persistencia.Repositorios.Base.SGP
{
    public abstract class RepositorioSgpBase
    {
        protected NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);
    }
}