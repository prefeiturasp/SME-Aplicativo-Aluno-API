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
                    (usuario_id,
                     notificacao_id,
                           criadoem,
                    codigo_eol_aluno, 
                      dre_codigoeol, 
                       ue_codigoeol, 
                        usuario_cpf,
                          criadopor,
                     mensagemvisualizada,
                     mensagemexcluida
                        )
                    VALUES(@UsuarioId,
                           @NotificacaoId,
                           @dataAtual,
                           @CodigoEolAluno,
                           @DreCodigoEol,
                           @UeCodigoEol,
                           @UsuarioCpf,
                           @CriadoPor,
                           @MensagemVisualizada,
                           @MensagemExcluida
                        );",
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
                        usuarioNotificacao.MensagemVisualizada,
                        usuarioNotificacao.MensagemExcluida
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


        public async Task<UsuarioNotificacao> ObterPorUsuarioAlunoNotificacao(long usuarioId, long codigoAluno, long notificacaoId)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);

            await conn.OpenAsync();

            var usuarioNotificacao = await conn.QueryFirstOrDefaultAsync<UsuarioNotificacao>(
                @"
                    SELECT 
                    	id, 
                    	usuario_id usuarioid, 
                    	codigo_eol_aluno codigoeolaluno, 
                    	notificacao_id notificacaoid, 
                    	criadoem, 
                    	dre_codigoeol drecodigoeol, 
                    	ue_codigoeol uecodigoeol, 
                    	usuario_cpf usuariocpf, 
                    	alteradoem, 
                    	criadopor, 
                    	alteradopor, 
                    	mensagemvisualizada, 
                    	mensagemexcluida
                    FROM usuario_notificacao_leitura
                    where
                        usuario_id = @usuario_id and
                        codigo_eol_aluno = @codigo_eol_aluno and
                        notificacao_id = @notificacaoId
                ", new { usuario_id = usuarioId, codigo_eol_aluno = codigoAluno, notificacaoId });

            await conn.CloseAsync();

            return usuarioNotificacao;
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
                             mensagemVisualizada= @MensagemVisualizada,
                             mensagemExcluida= @MensagemExcluida
                         WHERE id = @Id ;",
                new
                {
                    dataAtual,
                    usuarioNotificacao.UsuarioId,
                    usuarioNotificacao.MensagemVisualizada,
                    usuarioNotificacao.MensagemExcluida,
                    usuarioNotificacao.Id

                });
            conn.Close();
            return true;
        }

        public async Task<long> ObterTotalNotificacoesLeituraPorResponsavel(long notificacaoId, long dreCodigoEol, string ueCodigoEol)
        {
            try
            {
                var query = @"select count(distinct usuario_cpf) from usuario_notificacao_leitura unl where notificacao_id = @notificacaoId";
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var totalNotificacoesLeituraPorReponsavel = await conn.QuerySingleAsync<long>(query, new { notificacaoId, dreCodigoEol, ueCodigoEol });
                conn.Close();
                return totalNotificacoesLeituraPorReponsavel;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<long> ObterTotalNotificacoesLeituraPorAluno(long notificacaoId, long dreCodigoEol, string ueCodigoEol)
        {
            try
            {
                var query = @"select count(distinct codigo_eol_aluno) from usuario_notificacao_leitura unl where notificacao_id = @notificacaoId";
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var totalNotificacoesLeituraPorAluno = await conn.QuerySingleAsync<long>(query, new { notificacaoId, dreCodigoEol, ueCodigoEol });
                conn.Close();
                return totalNotificacoesLeituraPorAluno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}