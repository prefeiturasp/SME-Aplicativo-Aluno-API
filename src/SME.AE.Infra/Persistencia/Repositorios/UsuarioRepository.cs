
using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            Usuario usuario;

            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var resultado = await conn.QueryAsync<Usuario>(UsuarioConsultas.ObterPorCpf, new
                {
                    Cpf = cpf
                });
                usuario = resultado.FirstOrDefault();
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

            return usuario;
        }

        public async Task Criar(Usuario usuario)
        {
            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                await conn.ExecuteAsync(
                    @"INSERT INTO usuario( cpf, nome, email, ultimoLogin, criadoEm, excluido) 
                            VALUES(@Cpf, @Nome, @Email, @UltimoLogin,@UltimoLogin, @Excluido)",
                  usuario);
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }

        public async Task AtualizaUltimoLoginUsuario(string cpf)
        {
            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    "update usuario set ultimologin = @dataHoraAtual, excluido = false  where cpf = @cpf", new { cpf, dataHoraAtual });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }


        public async Task ExcluirUsuario(string cpf)
        {
            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    "update usuario set excluido = true , ultimoLogin = @dataHoraAtual  where cpf = @cpf", new { cpf, dataHoraAtual });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}
