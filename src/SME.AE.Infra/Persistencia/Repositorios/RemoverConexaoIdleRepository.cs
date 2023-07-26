using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class RemoverConexaoIdleRepository : IRemoverConexaoIdleRepository
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public RemoverConexaoIdleRepository(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions;
            this.servicoTelemetria = servicoTelemetria;
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

                var parametros = new { databaseName = conexao.Database };

                await servicoTelemetria.RegistrarAsync(async () =>
                    await SqlMapper.ExecuteAsync(conexao, sqlQuery.ToString(), parametros), "query", "Query AE", sqlQuery.ToString(), parametros.ToString());

                SentrySdk.CaptureMessage("Limpando pool de conexões idle do banco.");
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
