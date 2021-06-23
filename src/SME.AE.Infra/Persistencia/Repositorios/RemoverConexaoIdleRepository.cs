using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class RemoverConexaoIdleRepository : IRemoverConexaoIdleRepository
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public RemoverConexaoIdleRepository(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions;
        }
        public async Task RemoverConexoesIdle()
        {
            try
            {
                var conexao = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                await conexao.OpenAsync();

                var sqlQuery = new StringBuilder();
                sqlQuery.AppendLine("select pg_terminate_backend(pid)");
                sqlQuery.AppendLine("	from pg_stat_activity");
                sqlQuery.AppendLine("where datname = @databaseName and");
                sqlQuery.AppendLine(@"	  ""state"" = 'idle' and");
                sqlQuery.AppendLine("	  pid <> pg_backend_pid();");

                await conexao.ExecuteAsync(sqlQuery.ToString(), new { databaseName = conexao.Database });


                SentrySdk.CaptureMessage("Limpando pool de conexões idle do banco.");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
