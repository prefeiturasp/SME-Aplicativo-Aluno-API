using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Infra.Persistencia.Comandos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioCoreSSORepositorio : IUsuarioCoreSSORepositorio
    {
        public async Task Criar(UsuarioCoreSSO usuario)
        {
            try
            {
                using var conn = new SqlConnection(ConnectionStrings.ConexaoCoreSSO);
                conn.Open();
                using var transaction = conn.BeginTransaction();

                var pessoaId = Guid.NewGuid();
                var usuId = Guid.NewGuid();
                var parametrosPessoa = new { pessoaId, pesNome = usuario.Nome.Trim() };
                var parametrosUsuario = new {  usuId, login = usuario.Cpf.Trim(), senha = usuario.Senha,  pessoaId };
                var parametrosPessoaDoc = new { pesId = pessoaId, cpf = usuario.Cpf.Trim() };

                await conn.ExecuteAsync(CoreSSOComandos.InserirPessoa, parametrosPessoa, transaction);
                await conn.ExecuteAsync(CoreSSOComandos.InserirUsuario, parametrosUsuario, transaction);
                await conn.ExecuteAsync(CoreSSOComandos.InserirPessoaDocumento, parametrosPessoaDoc, transaction);

                transaction.Commit();
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public async Task<IEnumerable<RetornoUsuarioCoreSSO>> Selecionar(string cpf)
        {
            try
            {
                using var conn = new SqlConnection(ConnectionStrings.ConexaoCoreSSO);
                conn.Open();
                var resultado = await conn.QueryAsync<RetornoUsuarioCoreSSO>("SELECT u.usu_id usuId FROM sys_usuario u WHERE u.usu_login = @cpf", new { cpf });
                conn.Close();
                return resultado;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }
    }
}
