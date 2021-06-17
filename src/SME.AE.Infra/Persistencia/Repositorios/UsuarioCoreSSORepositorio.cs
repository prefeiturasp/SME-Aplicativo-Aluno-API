using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Comandos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioCoreSSORepositorio : IUsuarioCoreSSORepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public UsuarioCoreSSORepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        public async Task AlterarStatusUsuario(Guid usuId, StatusUsuarioCoreSSO novoStatus)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                conn.Open();
                using var transaction = conn.BeginTransaction();

                int status = (int)novoStatus;

                await conn.ExecuteAsync(CoreSSOComandos.AtualizarStatusUsuario, new { usuId, status }, transaction);
                await conn.ExecuteAsync(CoreSSOComandos.AtualizarStatusUsuarioGrupo, new { usuId, status }, transaction);

                transaction.Commit();
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task AlterarSenha(Guid usuarioId, string senhaCriptografada)
        {
            try
            {
                using var conexao = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                conexao.Open();
                var sql = @"update SYS_Usuario 
                               set usu_senha = @senhaCriptografada, usu_dataAlteracaoSenha = @dataAtual, usu_dataAlteracao = @dataAtual
                                where usu_id = @usuarioId;";

                await conexao.ExecuteAsync(sql, new { usuarioId, senhaCriptografada, dataAtual = DateTime.Now });
                conexao.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public async Task AtualizarCriptografiaUsuario(Guid usuId, string senha)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                conn.Open();
                await conn.ExecuteAsync(CoreSSOComandos.AtualizarCriptografia, new { usuId, senha });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public async Task<Guid> Criar(UsuarioCoreSSODto usuario)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                conn.Open();
                using var transaction = conn.BeginTransaction();

                var pessoaId = Guid.NewGuid();
                var usuId = Guid.NewGuid();
                var parametrosPessoa = new { pessoaId, pesNome = usuario.Nome.Trim() };
                var parametrosUsuario = new { usuId, login = usuario.Cpf.Trim(), senha = usuario.SenhaCriptografada, pessoaId };
                var parametrosPessoaDoc = new { pessoaId, cpf = usuario.Cpf.Trim() };

                await conn.ExecuteAsync(CoreSSOComandos.InserirPessoa, parametrosPessoa, transaction);
                await conn.ExecuteAsync(CoreSSOComandos.InserirUsuario, parametrosUsuario, transaction);
                await conn.ExecuteAsync(CoreSSOComandos.InserirPessoaDocumento, parametrosPessoaDoc, transaction);

                foreach (var grupo in usuario.Grupos)
                    await conn.ExecuteAsync(CoreSSOComandos.InserirUsuarioGrupo, new { gruId = grupo, usuId }, transaction);

                transaction.Commit();
                conn.Close();

                return usuId;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public async Task IncluirUsuarioNosGrupos(Guid usuId, IEnumerable<Guid> gruposNaoIncluidos)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);

                conn.Open();
                using var transaction = conn.BeginTransaction();

                foreach (var grupo in gruposNaoIncluidos)
                    await conn.ExecuteAsync(CoreSSOComandos.InserirUsuarioGrupo, new { gruId = grupo, usuId }, transaction);

                transaction.Commit();
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<RetornoUsuarioCoreSSO> ObterPorId(Guid id)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                conn.Open();

                var consulta = @"
                    SELECT u.usu_id usuId,u.usu_senha as senha, u.usu_situacao as status, u.usu_criptografia as TipoCriptografia, u.usu_login as Cpf
                    FROM sys_usuario u
                        WHERE u.usu_id = @id";

                return await conn.QueryFirstOrDefaultAsync<RetornoUsuarioCoreSSO>(consulta, new { id });
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<RetornoUsuarioCoreSSO> ObterPorCPF(string cpf)
        {
            try
            {
                RetornoUsuarioCoreSSO usuarioCoreSSO;

                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);

                conn.Open();
                usuarioCoreSSO = await conn.QueryFirstOrDefaultAsync<RetornoUsuarioCoreSSO>(@"
                            SELECT u.usu_id usuId,u.usu_senha as senha, u.usu_situacao as status, u.usu_criptografia as TipoCriptografia, u.usu_login as Cpf
                            FROM sys_usuario u
                            WHERE u.usu_login = @cpf "
                    , new { cpf });
                conn.Close();


                return usuarioCoreSSO;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureMessage(ex.Message, Sentry.Protocol.SentryLevel.Error);
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<List<Guid>> SelecionarGrupos()
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                conn.Open();
                var listaIdGrupoQry = await conn.QueryAsync<Guid>(@"
                    SELECT gru_id
                    FROM sys_grupo 
                        WHERE sis_id = 1001");
                conn.Close();
                return listaIdGrupoQry.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }
    }
}
