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
                     (usuario_id, codigo_eol_aluno, notificacao_id, criadoem)
                    VALUES(@usuarioId, 0, @notificacaoId, @dataAtual);", new { usuarioNotificacao.UsuarioId, usuarioNotificacao.NotificacaoId, dataAtual });
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }

        public Task<UsuarioNotificacao> ObterPorId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Remover(long notificacaoId)
        {
            bool resultado = false;

            try
            {
                await using (var conn = new Npgsql.NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();

                    var retorno = await conn.ExecuteAsync(
                        @"DELETE FROM usuario_notificacao_leitura where id = @ID", new { notificacaoId });
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return resultado;
            }

            return resultado;
        }
    }
}
