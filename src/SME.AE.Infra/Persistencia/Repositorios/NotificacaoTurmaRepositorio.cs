using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
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

        public async Task<bool> RemoverPorNotificacaoId(long id)
        {
            return await RemoverPorNotificacoesIds([id]);
        }

        public async Task<bool> RemoverPorNotificacoesIds(long[] id)
        {
            await using (var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync(
                    @"UPDATE notificacao_turma SET excluido = true where notificacao_id = ANY(@notificacoesIds)",
                    new { id });
                await conn.CloseAsync();
            }
            return true;
        }
    }
}