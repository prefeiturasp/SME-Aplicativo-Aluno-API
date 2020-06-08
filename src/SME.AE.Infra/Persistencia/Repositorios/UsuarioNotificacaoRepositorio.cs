using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioNotificacaoRepositorio : IUsuarioNotificacaoRepositorio
    {

        public async Task<bool> Criar(UsuarioNotificacao usuarioNotificacao)
        {

            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataAtual = DateTime.Now;
                var retorno = await conn.ExecuteAsync(
                    @"INSERT INTO public.usuario_notificacao_leitura
                     (usuario_id,  notificacao_id, criadoem, mensagemLida, ueId, dreId)
                    VALUES(@usuarioId, @notificacaoId, @dataAtual, @mensagemLida, @ueId, @dreId);",
                     new
                     {
                         usuarioNotificacao.UsuarioId,
                         usuarioNotificacao.NotificacaoId,
                         dataAtual,
                         usuarioNotificacao.MensagemLida,
                         usuarioNotificacao.UeId,
                         usuarioNotificacao.DreId
                     });
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw new Exception("Erro ao marcar a mensagem como  lida, tente novamente");
            }
        }

        public Task<UsuarioNotificacao> ObterPorId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoverPorId(long id)
        {


            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();

                await conn.ExecuteAsync(
                   @"DELETE FROM usuario_notificacao_leitura where id = @id", new { id });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw new Exception("Erro ao marcar a mensagem como não lida, tente novamente");
            }

            return true;
        }

        public async Task<bool> Remover(long notificacaoId)
        {


            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();

                await conn.ExecuteAsync(
                   @"DELETE FROM usuario_notificacao_leitura where notificacao_id = @notificacaoId", new { notificacaoId });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }

            return true;
        }

        public async Task<UsuarioNotificacao> Selecionar(UsuarioNotificacao usuarioNotificacao)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var dataAtual = DateTime.Now;
            var retorno = await conn.QueryFirstOrDefaultAsync<UsuarioNotificacao>(
                @"SELECT * from public.usuario_notificacao_leitura
                     WHERE usuario_id = @UsuarioId AND notificacao_id = @NotificacaoId", new { usuarioNotificacao.UsuarioId, usuarioNotificacao.NotificacaoId });
            conn.Close();
            return retorno;
        }
    }
}
