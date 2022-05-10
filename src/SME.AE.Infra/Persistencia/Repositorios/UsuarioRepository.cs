using Dapper;
using Dapper.Dommel;
using Dommel;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioRepository : BaseRepositorio<Usuario>, IUsuarioRepository
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
        private readonly IServicoTelemetria servicoTelemetria;

        public UsuarioRepository(VariaveisGlobaisOptions variaveisGlobaisOptions,
            IServicoTelemetria servicoTelemetria) : base(variaveisGlobaisOptions.AEConnection)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
            this.servicoTelemetria = servicoTelemetria;
        }

        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            try
            {
                using var conexao = InstanciarConexao();

                conexao.Open();

                var sql = "select * from usuario where cpf = @cpf";
                var parametros = new { cpf };

                var usuario = await servicoTelemetria.RegistrarComRetornoAsync<Usuario>(async () =>
                    await SqlMapper.QueryFirstOrDefaultAsync<Usuario>(conexao, sql, parametros), "query", "Query AE", sql, parametros.ToString());

                return usuario;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Usuario>> ObterTodosUsuariosAtivos()
        {
            var query = "select * from usuario where excluido = false";

            try
            {
                using var conexao = InstanciarConexao();

                await conexao.OpenAsync();

                var usuarios = await servicoTelemetria.RegistrarComRetornoAsync<Usuario>(async () =>
                    await SqlMapper.QueryAsync<Usuario>(conexao, query), "query", "Query AE", query);

                await conexao.CloseAsync();

                return usuarios;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task<Usuario> ObterUsuarioNaoExcluidoPorCpf(string cpf)
        {
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var usuario = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Cpf == cpf);
                return usuario;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task<IEnumerable<string>> ObterTodos()
        {
            try
            {
                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                conn.Open();

                var cpfsUsuarios = await servicoTelemetria.RegistrarComRetornoAsync<string>(async () =>
                    await SqlMapper.QueryAsync<string>(conn, UsuarioConsultas.ObterTodos), "query", "Query AE", UsuarioConsultas.ObterTodos);

                conn.Close();

                return cpfsUsuarios;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task<long> ObterTotalUsuariosComAcessoIncompleto(List<string> cpfs)
        {
            try
            {
                var cpfsIN = "'" + string.Join<string>("','", cpfs) + "'";
                var query = new StringBuilder();
                query.AppendLine($"{UsuarioConsultas.ObterTotalUsuariosComAcessoIncompleto}");

                if (cpfs != null && cpfs.Any())
                    query.AppendLine($" and cpf IN ({cpfsIN})");

                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                conn.Open();

                var totalUsuariosComAcessoIncompleto = await servicoTelemetria.RegistrarComRetornoAsync<long>(async () =>
                    await SqlMapper.ExecuteScalarAsync<long>(conn, query.ToString()), "query", "Query AE", query.ToString());

                conn.Close();

                return (long)totalUsuariosComAcessoIncompleto;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return 0;
            }
        }

        public async Task<long> ObterTotalUsuariosValidos(List<string> cpfs)
        {
            try
            {
                var cpfsIN = "'" + string.Join<string>("','", cpfs) + "'";

                var query = new StringBuilder();
                query.AppendLine($"{UsuarioConsultas.ObterTotalUsuariosValidos}");

                if (cpfs != null && cpfs.Any())
                    query.AppendLine($" and cpf IN ({cpfsIN})");

                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                conn.Open();

                var totalUsuariosValidos = await servicoTelemetria.RegistrarComRetornoAsync<long>(async () =>
                    await SqlMapper.ExecuteScalarAsync<long>(conn, query.ToString()), "query", "Query AE", query.ToString());

                conn.Close();

                return (long)totalUsuariosValidos;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return 0;
            }
        }

        public async Task AtualizaUltimoLoginUsuario(string cpf)
        {
            try
            {
                var dataHoraAtual = DateTime.Now;
                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
                conn.Open();
                await conn.ExecuteAsync(
                    "update usuario set ultimologin = @dataHoraAtual, excluido = false  where cpf = @cpf", new { cpf, dataHoraAtual });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task AltualizarUltimoAcessoPrimeiroUsuario(Usuario usuario)
        {
            try
            {
                var sql = @"UPDATE usuario
                SET ultimologin=@ultimologin, excluido=@Excluido, primeiroacesso=@PrimeiroAcesso, 
                alteradoem=@AlteradoEm, alteradopor=@AlteradoPor, token_redefinicao = '', redefinicao = false, validade_token = null
                where id=@Id;";

                using var conexao = InstanciarConexao();
                conexao.Open();
                await conexao.ExecuteAsync(sql, usuario);
                conexao.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task AtualizarEmailTelefone(long id, string email, string celular)
        {
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(@"UPDATE usuario SET alteradopor='Sistema', alteradoem=@alteradoem");

                if (!string.IsNullOrWhiteSpace(email))
                    builder.AppendLine(",email=@email");

                if (!string.IsNullOrWhiteSpace(celular))
                    builder.AppendLine(",celular=@celular");

                builder.AppendLine("where id = @id;");

                using var conexao = InstanciarConexao();
                await conexao.ExecuteAsync(builder.ToString(), new { id, email, celular, alteradoem = DateTime.Now });
                conexao.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task AtualizarPrimeiroAcesso(long id, bool primeiroAcesso)
        {
            try
            {
                var conexao = InstanciarConexao();
                await conexao.ExecuteAsync(@"UPDATE usuario
                            SET primeiroacesso=@primeiroAcesso,alteradopor='Sistema', alteradoem=@alteradoem
                            where id = @id;", new { id, primeiroAcesso, alteradoem = DateTime.Now });
                conexao.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task<Usuario> ObterUsuarioPorTokenAutenticacao(string token)
        {
            try
            {
                using var conexao = InstanciarConexao();
                conexao.Open();
                var usuario = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Token == token && x.RedefinirSenha);
                conexao.Close();
                return usuario;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task ExcluirUsuario(string cpf)
        {
            try
            {
                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    "update usuario set excluido = true , ultimoLogin = @dataHoraAtual, token_redefinicao = '', redefinicao = false, validade_token = null  where cpf = @cpf", new { cpf, dataHoraAtual });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task CriaUsuarioDispositivo(long usuarioId, string dispositivoId)
        {
            try
            {
                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    @"INSERT INTO public.usuario_dispositivo
                          (usuario_id, codigo_dispositivo, criadoem)
                           VALUES(@usuarioId, @dispositivoId , @dataHoraAtual ); ", new { usuarioId, dispositivoId, dataHoraAtual });
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }

        public async Task<bool> RemoveUsuarioDispositivo(long usuarioId, string dispositivoId)
        {
            try
            {
                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);
                conn.Open();
                await conn.ExecuteAsync(
                    @"DELETE FROM public.usuario_dispositivo
                            WHERE usuario_id = @usuarioId  AND codigo_dispositivo = @dispositivoId ;", new { usuarioId, dispositivoId });
                conn.Close();

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
                using var conn = new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

                conn.Open();

                string query =
                     @"SELECT usuario_id 
                            FROM public.usuario_dispositivo
                           WHERE usuario_id = @usuarioId
                             AND codigo_dispositivo =  @dispositivoId";

                var parametros = new { usuarioId, dispositivoId };

                var retorno = await servicoTelemetria.RegistrarComRetornoAsync<bool>(async () =>
                    await SqlMapper.QueryAsync<bool>(conn, query, parametros), "query", "Query AE", query, parametros.ToString());

                if (!retorno.Any())
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }

        public override async Task<Usuario> ObterPorIdAsync(long id)
        {
            return await base.ObterPorIdAsync(id);
        }
        public override async Task RemoverAsync(long id)
        {
            await base.RemoverAsync(id);
        }
        public override async Task RemoverAsync(Usuario usuario)
        {
            await base.RemoverAsync(usuario);
        }
        public override async Task<long> SalvarAsync(Usuario usuario)
        {
            return await base.SalvarAsync(usuario);
        }
    }
}