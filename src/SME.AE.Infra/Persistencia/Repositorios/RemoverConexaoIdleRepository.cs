using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class RemoverConexaoIdleRepository : IRemoverConexaoIdleRepository
    {
        public void RemoverConexoesIdle()
        {
            NpgsqlConnection.ClearAllPools();
        }
    }
}
