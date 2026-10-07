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
using Microsoft.Data.SqlClient;
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
                await conn.OpenAsync();
                using var transaction = conn.BeginTransaction();

                int status = (int)novoStatus;

                await conn.ExecuteAsync(CoreSSOComandos.AtualizarStatusUsuario, new { usuId, status }, transaction);
                await conn.ExecuteAsync(CoreSSOComandos.AtualizarStatusUsuarioGrupo, new { usuId, status }, transaction);

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task AlterarSenha(Guid usuarioId, string senhaCriptografada)
        {
            try
            {
                using var conexao = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                await conexao.OpenAsync();
                var sql = @"update SYS_Usuario 
                               set usu_senha = @senhaCriptografada, usu_dataAlteracaoSenha = @dataAtual, usu_dataAlteracao = @dataAtual
                                where usu_id = @usuarioId;";

                await conexao.ExecuteAsync(sql, new { usuarioId, senhaCriptografada, dataAtual = DateTime.Now });

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task AtualizarCriptografiaUsuario(Guid usuId, string senha)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                await conn.OpenAsync();
                await conn.ExecuteAsync(CoreSSOComandos.AtualizarCriptografia, new { usuId, senha });

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task<Guid> Criar(UsuarioCoreSSODto usuario)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                await conn.OpenAsync();
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

                await transaction.CommitAsync();


                return usuId;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task IncluirUsuarioNosGrupos(Guid usuId, IEnumerable<Guid> gruposNaoIncluidos)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);

                await conn.OpenAsync();
                using var transaction = conn.BeginTransaction();

                foreach (var grupo in gruposNaoIncluidos)
                    await conn.ExecuteAsync(CoreSSOComandos.InserirUsuarioGrupo, new { gruId = grupo, usuId }, transaction);

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task<RetornoUsuarioCoreSSO> ObterPorId(Guid id)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                await conn.OpenAsync();

                var consulta = @"
                    SELECT u.usu_id usuId,u.usu_senha as senha, u.usu_situacao as status, u.usu_criptografia as TipoCriptografia, u.usu_login as Cpf
                    FROM sys_usuario u
                        WHERE u.usu_id = @id";

                return await conn.QueryFirstOrDefaultAsync<RetornoUsuarioCoreSSO>(consulta, new { id });
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task<RetornoUsuarioCoreSSO> ObterPorCPF(string cpf)
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);

                await conn.OpenAsync();
                return await conn.QueryFirstOrDefaultAsync<RetornoUsuarioCoreSSO>(@"
                            SELECT u.usu_id usuId,u.usu_senha as senha, u.usu_situacao as status, u.usu_criptografia as TipoCriptografia, u.usu_login as Cpf
                            FROM sys_usuario u
                            WHERE u.usu_login = @cpf "
                    , new { cpf });
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }

        public async Task<List<Guid>> SelecionarGrupos()
        {
            try
            {
                using var conn = new SqlConnection(variaveisGlobaisOptions.CoreSSOConnection);
                await conn.OpenAsync();
                var listaIdGrupoQry = await conn.QueryAsync<Guid>(@"
                    SELECT gru_id
                    FROM sys_grupo 
                        WHERE sis_id = 1001");

                return listaIdGrupoQry.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw;
            }
        }
    }
}
