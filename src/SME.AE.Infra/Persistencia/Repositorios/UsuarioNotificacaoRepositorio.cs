using Dapper;
using Npgsql;
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
                    (usuario_id,
                     notificacao_id,
                           criadoem,
                    codigo_eol_aluno, 
                      dre_codigoeol, 
                       ue_codigoeol, 
                        usuario_cpf,
                          criadopor,
                     mensagemvisualizada)
                    VALUES(@UsuarioId,
                           @NotificacaoId,
                           @dataAtual,
                           @CodigoEolAluno,
                           @DreCodigoEol,
                           @UeCodigoEol,
                           @UsuarioCpf,
                           @CriadoPor,
                           @MensagemVisualizada);",
                    new
                    {
                        usuarioNotificacao.UsuarioId,
                        usuarioNotificacao.NotificacaoId,
                        dataAtual,
                        usuarioNotificacao.CodigoEolAluno,
                        usuarioNotificacao.DreCodigoEol,
                        usuarioNotificacao.UeCodigoEol,
                        usuarioNotificacao.UsuarioCpf,
                        usuarioNotificacao.CriadoPor,
                        usuarioNotificacao.MensagemVisualizada
                    });
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }

        public Task<UsuarioNotificacao> ObterPorId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoverPorId(long id)
        {

            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();

            await conn.ExecuteAsync(
               @"DELETE FROM usuario_notificacao_leitura where id = @id", new { id });
            conn.Close();

            return true;
        }

        public async Task<bool> Remover(long notificacaoId)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();

            await conn.ExecuteAsync(
               @"DELETE FROM usuario_notificacao_leitura where notificacao_id = @notificacaoId", new { notificacaoId });
            conn.Close();

            return true;
        }

        public async Task<UsuarioNotificacao> Selecionar(UsuarioNotificacao usuarioNotificacao)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var dataAtual = DateTime.Now;
            var retorno = await conn.QueryFirstOrDefaultAsync<UsuarioNotificacao>(
                @"SELECT id, usuario_id UsuarioId, notificacao_id NotificacaoId from public.usuario_notificacao_leitura
                     WHERE usuario_id = @UsuarioId AND notificacao_id = @NotificacaoId", new { usuarioNotificacao.UsuarioId, usuarioNotificacao.NotificacaoId });
            conn.Close();
            return retorno;
        }


        public async Task<UsuarioNotificacao> ObterPorNotificacaoIdEhUsuarioCpf(long notificacaoId, string usuarioCpf, long dreCodigoEol, string ueCodigoEol)
        {
            var query = @"select
	                        *
                        from
	                        public.usuario_notificacao_leitura
                        where
	                        usuario_cpf = @usuarioCpf
	                        and notificacao_id = @notificacaoId
	                        and dre_codigoeol = @dreCodigoEol
	                        and ue_codigoeol = @ueCodigoEol";

            UsuarioNotificacao retorno = null;
            await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
            {

                conn.Open();
                retorno = await conn.QueryFirstOrDefaultAsync<UsuarioNotificacao>(
                    query, new { usuarioCpf, notificacaoId, dreCodigoEol, ueCodigoEol });
                conn.Close();
            }
            return retorno;
        }

        public async Task<bool> Atualizar(UsuarioNotificacao usuarioNotificacao)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var dataAtual = DateTime.Now;
            var retorno = await conn.ExecuteAsync(
                @"UPDATE public.usuario_notificacao_leitura
                         SET 
                             alteradoem= @dataAtual, 
                             alteradopor= @UsuarioId, 
                             mensagemVisualizada= @MensagemVisualizada 
                         WHERE id = @Id ;",
                new
                {
                    dataAtual,
                    usuarioNotificacao.UsuarioId,
                    usuarioNotificacao.MensagemVisualizada,
                    usuarioNotificacao.Id

                });
            conn.Close();
            return true;
        }
    }
}