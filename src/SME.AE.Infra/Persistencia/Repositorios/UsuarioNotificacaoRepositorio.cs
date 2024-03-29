﻿using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioNotificacaoRepositorio : IUsuarioNotificacaoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public UsuarioNotificacaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<bool> Criar(UsuarioNotificacao usuarioNotificacao)
        {
            try
            {
                await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
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
            await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
            conn.Open();

            await conn.ExecuteAsync(
               @"UPDATE FROM usuario_notificacao_leitura SET excluido = true where id = @id", new { id });

            conn.Close();

            return true;
        }

        public async Task<UsuarioNotificacao> ObterPorUsuarioAlunoNotificacao(long usuarioId, long codigoAluno, long notificacaoId)
        {
            using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

            await conn.OpenAsync();

            var sql = @"SELECT 
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
                        notificacao_id = @notificacaoId and not excluido";

            var parametros = new { usuario_id = usuarioId, codigo_eol_aluno = codigoAluno, notificacaoId };

            var usuarioNotificacao = await servicoTelemetria.RegistrarComRetornoAsync<UsuarioNotificacao>(async () =>
                await SqlMapper.QueryFirstOrDefaultAsync<UsuarioNotificacao>(conn, sql, parametros), "query", "Query AE", sql, parametros.ToString());

            await conn.CloseAsync();

            return usuarioNotificacao;
        }

        public async Task<bool> RemoverPorNotificacaoId(long notificacaoId)
        {
            await using (var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection))
            {
                conn.Open();
                await conn.ExecuteAsync(
               @"UPDATE usuario_notificacao_leitura SET excluido = true where notificacao_id = @notificacaoId", new { notificacaoId });
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
               @"UPDATE usuario_notificacao_leitura SET excluido = true where notificacao_id = ANY(@notificacoesIds)", new { notificacoesIds });
                conn.Close();
            }
            return true;
        }

        public async Task<UsuarioNotificacao> Selecionar(UsuarioNotificacao usuarioNotificacao)
        {
            using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

            conn.Open();

            var sql = @"SELECT id, usuario_id UsuarioId, notificacao_id NotificacaoId from public.usuario_notificacao_leitura
                     WHERE usuario_id = @UsuarioId AND notificacao_id = @NotificacaoId and not excluido";

            var parametros = new { usuarioNotificacao.UsuarioId, usuarioNotificacao.NotificacaoId };

            var retorno = await servicoTelemetria.RegistrarComRetornoAsync<UsuarioNotificacao>(async () =>
                await SqlMapper.QueryFirstOrDefaultAsync<UsuarioNotificacao>(conn, sql, parametros), "query", "Query AE", sql, parametros.ToString());

            conn.Close();

            return retorno;
        }

        public async Task<UsuarioNotificacao> ObterPorNotificacaoIdEhUsuarioCpf(long notificacaoId, string usuarioCpf, long dreCodigoEol, string ueCodigoEol, long codigoEolAluno)
        {
            var query = @"select
                            id as Id,
	                        usuario_Id as UsuarioId,
                            codigo_eol_aluno as CodigoEolAluno,
                            notificacao_id as NotificacaoId,
                            dre_codigoeol as DreCodigoEol,
                            ue_codigoeol as UeCodigoEol,
                            usuario_cpf as UsuarioCpf
                        from
	                        public.usuario_notificacao_leitura
                        where
	                        usuario_cpf = @usuarioCpf
                            and codigo_eol_aluno = @codigoEolAluno
	                        and notificacao_id = @notificacaoId
	                        and dre_codigoeol = @dreCodigoEol
	                        and ue_codigoeol = @ueCodigoEol and not excluido";

            var parametros = new { usuarioCpf, notificacaoId, dreCodigoEol, ueCodigoEol, codigoEolAluno };

            UsuarioNotificacao retorno = null;

            await using (var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection))
            {
                conn.Open();

                retorno = await servicoTelemetria.RegistrarComRetornoAsync<UsuarioNotificacao>(async () =>
                    await SqlMapper.QueryFirstOrDefaultAsync<UsuarioNotificacao>(conn, query, parametros), "query", "Query AE", query, parametros.ToString());

                conn.Close();
            }

            return retorno;
        }

        public async Task<bool> Atualizar(UsuarioNotificacao usuarioNotificacao)
        {
            await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

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

        public async Task<long> ObterTotalNotificacoesLeituraPorResponsavel(long notificacaoId, long codigoDre)
        {
            try
            {
                var query = new StringBuilder();
                query.AppendLine(@"select count(distinct usuario_cpf) from usuario_notificacao_leitura unl where notificacao_id = @notificacaoId and not excluido ");

                if (codigoDre > 0)
                    query.AppendLine(" and dre_codigoeol = @codigoDre ");

                var parametros = new { notificacaoId, codigoDre };

                await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                conn.Open();

                var totalNotificacoesLeituraPorReponsavel = await servicoTelemetria.RegistrarComRetornoAsync<long>(async () =>
                    await SqlMapper.QuerySingleAsync<long>(conn, query.ToString(), parametros), "query", "Query AE", query.ToString(), parametros.ToString());

                conn.Close();

                return totalNotificacoesLeituraPorReponsavel;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<long> ObterTotalNotificacoesLeituraPorAluno(long notificacaoId, long codigoDre)
        {
            try
            {
                var query = new StringBuilder();
                query.AppendLine(@"select count(distinct codigo_eol_aluno) from usuario_notificacao_leitura unl where notificacao_id = @notificacaoId and not excluido ");

                var parametros = new { notificacaoId, codigoDre };

                if (codigoDre > 0)
                    query.AppendLine(" and dre_codigoeol = @codigoDre ");

                await using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                conn.Open();

                var totalNotificacoesLeituraPorAluno = await servicoTelemetria.RegistrarComRetornoAsync<long>(async () =>
                    await SqlMapper.QuerySingleAsync<long>(conn, query.ToString(), parametros), "query", "Query AE", query.ToString(), parametros.ToString());

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