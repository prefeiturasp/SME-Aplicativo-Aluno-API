using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class RemoverConexaoIdleRepository : IRemoverConexaoIdleRepository
    {
        public void RemoverConexoesIdle()
        {
            try
            {
                NpgsqlConnection.ClearAllPools();
                SentrySdk.CaptureMessage("Limpando pool de conexões idle do banco.");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
