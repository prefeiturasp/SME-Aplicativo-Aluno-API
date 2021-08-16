using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotificacaoTurmaRepositorio : INotificacaoTurmaRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public NotificacaoTurmaRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        public async Task<bool> RemoverPorNotificacaoId(long notificacaoId)
        {
            await using (var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection))
            {
                conn.Open();
                await conn.ExecuteAsync(
               @"UPDATE notificacao_turma SET excluido = true where notificacao_id = @notificacaoId", new { notificacaoId });
                conn.Close();
            }
            return true;
        }

        public async Task<bool> RemoverPorNotificacoesIds(long[] notificacoesIds)
        {
            await using (var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection))
            {
                conn.Open();
                await conn.ExecuteAsync(
               @"UPDATE notificacao_turma SET excluido = true where notificacao_id = ANY(@notificacoesIds)", new { notificacoesIds });
                conn.Close();
            }
            return true;
        }
    }
}
