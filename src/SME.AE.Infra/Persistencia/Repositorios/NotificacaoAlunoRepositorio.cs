using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotificacaoAlunoRepositorio : INotificacaoAlunoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public NotificacaoAlunoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        public Task<bool> RemoverPorNotificacaoId(long id) =>
            RemoverPorNotificacoesIds([id]);

        public Task<bool> RemoverPorNotificacoesIds(long[] id) =>
            ExecutarUpdateAsync(@"UPDATE notificacao_aluno SET excluido = true where notificacao_id = ANY(@notificacoesIds)", new { id });

        private async Task<bool> ExecutarUpdateAsync(string query, object parametros)
        {
            await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

            await conn.ExecuteAsync(query, parametros);

            return true;
        }
    }
}