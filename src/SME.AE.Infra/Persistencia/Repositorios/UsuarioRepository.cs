
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Dommel;
using Dommel;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioRepository : BaseRepositorio<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository() : base(ConnectionStrings.Conexao)
        {

        }

        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            using var conexao = InstanciarConexao();

            conexao.Open();

            var retorno = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Cpf == cpf);

            conexao.Close();

            return retorno;
        }

        public async Task<IEnumerable<string>> ObterTodos()
        {
            try
            {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var resultado = await conn.QueryAsync<string>(UsuarioConsultas.ObterTodos);
                conn.Close();
                return resultado;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

        }
                     
        public async Task AtualizaUltimoLoginUsuario(string cpf)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var dataHoraAtual = DateTime.Now;
            await conn.ExecuteAsync(
                "update usuario set ultimologin = @dataHoraAtual, excluido = false  where cpf = @cpf", new { cpf, dataHoraAtual });
            conn.Close();
        }

        public async Task AltualizarUltimoAcessoPrimeiroUsuario(Usuario usuario)
        {
            var sql = @"UPDATE usuario
                SET ultimologin=@ultimologin, excluido=@Excluido, primeiroacesso=@PrimeiroAcesso, 
                alteradoem=@AlteradoEm, alteradopor=@AlteradoPor, token_redefinicao = '', redefinicao = false, validade_token = null
                where id=@Id;";

            using (var conexao = InstanciarConexao())
            {
                await conexao.ExecuteAsync(sql, usuario);
            }
        }

        public async Task AtualizarEmailTelefone(long id, string email, string celular)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(@"UPDATE usuario SET alteradopor='Sistema', alteradoem=@alteradoem");

            if (!string.IsNullOrWhiteSpace(email))
                builder.AppendLine(",email=@email");

            if (!string.IsNullOrWhiteSpace(celular))
                builder.AppendLine(",celular=@celular");

            builder.AppendLine("where id = @id;");

            using (var conexao = InstanciarConexao())
            {
                await conexao.ExecuteAsync(builder.ToString(), new { id, email, celular, alteradoem = DateTime.Now });

                conexao.Close();
            }
        }

        public async Task AtualizarPrimeiroAcesso(long id, bool primeiroAcesso)
        {
            using (var conexao = InstanciarConexao())
            {
                await conexao.ExecuteAsync(@"UPDATE usuario
                            SET primeiroacesso=@primeiroAcesso,alteradopor='Sistema', alteradoem=@alteradoem
                            where id = @id;", new { id, primeiroAcesso, alteradoem = DateTime.Now });

                conexao.Close();
            }
        }

        public async Task<Usuario> ObterUsuarioPorTokenAutenticacao(string token)
        {
            using var conexao = InstanciarConexao();

            conexao.Open();

            var retorno = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Token == token && x.RedefinirSenha);

            conexao.Close();

            return retorno;
        }


        public async Task ExcluirUsuario(string cpf)
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var dataHoraAtual = DateTime.Now;
            await conn.ExecuteAsync(
                "update usuario set excluido = true , ultimoLogin = @dataHoraAtual, token_redefinicao = '', redefinicao = false, validade_token = null  where cpf = @cpf", new { cpf, dataHoraAtual });
            conn.Close();
        }


        public async Task CriaUsuarioDispositivo(long usuarioId, string dispositivoId)
        {
            await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
            {
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    @"INSERT INTO public.usuario_dispositivo
                          (usuario_id, codigo_dispositivo, criadoem)
                           VALUES(@usuarioId, @dispositivoId , @dataHoraAtual ); ", new { usuarioId, dispositivoId, dataHoraAtual });
                conn.Close();
            }
        }

        public async Task<bool> RemoveUsuarioDispositivo(long usuarioId, string dispositivoId)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    await conn.ExecuteAsync(
                        @"DELETE FROM public.usuario_dispositivo
                            WHERE usuario_id = @usuarioId  AND codigo_dispositivo = @dispositivoId ;", new { usuarioId, dispositivoId });
                    conn.Close();

                }
                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }

        public async Task<bool> ExisteUsuarioDispositivo(long usuarioId, string dispositivoId)
        {

            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    string query =
                         @"SELECT usuario_id 
                            FROM public.usuario_dispositivo
                           WHERE usuario_id = @usuarioId
                             AND codigo_dispositivo =  @dispositivoId";
                    var retorno = await conn.QueryAsync(query, new { usuarioId, dispositivoId });
                    if (retorno.Count() == 0)
                        return false;
                }

                return true;

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }
    }
}