
using Dapper;
using Dapper.Dommel;
using Dommel;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
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
        private const string USUARIOPORCPF = "UsuarioCpf";
        private const string USUARIOPORID = "UsuarioId";
        private const string USUARIOPORTOKEN = "UsuarioToken";

        private readonly ICacheRepositorio cacheRepositorio;
        public UsuarioRepository(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            try
            {
                var usuarioCache = ObterUsuarioCachePorCpf(cpf);
                if (usuarioCache != null) return usuarioCache;

                using var conexao = InstanciarConexao();
                conexao.Open();
                var usuario = await conexao.FirstOrDefaultAsync<Usuario>(x => x.Cpf == cpf);
                await SalvarUsuarioCache(usuario);
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
                
                var usuarios = await conexao.QueryAsync<Usuario>(query);
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
                var usuarioCache = ObterUsuarioCachePorCpf(cpf);
                if (usuarioCache != null) return usuarioCache;

                using var conexao = InstanciarConexao();
                conexao.Open();
                var usuario = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Cpf == cpf);
                await SalvarUsuarioCache(usuario);
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
                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var cpfsUsuarios = await conn.QueryAsync<string>(UsuarioConsultas.ObterTodos);
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

                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var totalUsuariosComAcessoIncompleto = await conn.ExecuteScalarAsync(query.ToString());
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

                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var totalUsuariosValidos = await conn.ExecuteScalarAsync(query.ToString());
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
                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                await conn.ExecuteAsync(
                    "update usuario set ultimologin = @dataHoraAtual, excluido = false  where cpf = @cpf", new { cpf, dataHoraAtual });
                conn.Close();
                await LimparUsuarioCachePorCpf(cpf);
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
                await LimparUsuarioCache(usuario);
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
                await LimparUsuarioCachePorId(id);
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
                await LimparUsuarioCachePorId(id);
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
                var usuarioCache = ObterUsuarioCachePorToken(token);
                if (usuarioCache != null)
                    return usuarioCache;

                using var conexao = InstanciarConexao();
                conexao.Open();
                var usuario = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Token == token && x.RedefinirSenha);
                conexao.Close();
                await SalvarUsuarioCache(usuario);
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
                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    "update usuario set excluido = true , ultimoLogin = @dataHoraAtual, token_redefinicao = '', redefinicao = false, validade_token = null  where cpf = @cpf", new { cpf, dataHoraAtual });
                conn.Close();
                await LimparUsuarioCachePorCpf(cpf);
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
                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataHoraAtual = DateTime.Now;
                await conn.ExecuteAsync(
                    @"INSERT INTO public.usuario_dispositivo
                          (usuario_id, codigo_dispositivo, criadoem)
                           VALUES(@usuarioId, @dispositivoId , @dataHoraAtual ); ", new { usuarioId, dispositivoId, dataHoraAtual });
                conn.Close();

                var chaveCache = $"UsuarioDispositivo-{usuarioId}-{dispositivoId}";
                await cacheRepositorio.SalvarAsync(chaveCache, "true", 1080, false);
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
                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                await conn.ExecuteAsync(
                    @"DELETE FROM public.usuario_dispositivo
                            WHERE usuario_id = @usuarioId  AND codigo_dispositivo = @dispositivoId ;", new { usuarioId, dispositivoId });
                conn.Close();

                var chaveCache = $"UsuarioDispositivo-{usuarioId}-{dispositivoId}";
                await cacheRepositorio.RemoverAsync(chaveCache);

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
                var chaveCache = $"UsuarioDispositivo-{usuarioId}-{dispositivoId}";
                var usuarioDispositivo = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(usuarioDispositivo))
                    return true;

                using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                string query =
                     @"SELECT usuario_id 
                            FROM public.usuario_dispositivo
                           WHERE usuario_id = @usuarioId
                             AND codigo_dispositivo =  @dispositivoId";
                var retorno = await conn.QueryAsync(query, new { usuarioId, dispositivoId });
                if (!retorno.Any())
                    return false;

                await cacheRepositorio.SalvarAsync(chaveCache, "true", 1080, false);
                return true;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return false;
            }
        }

        private Usuario ObterUsuarioCachePorId(long id)
            => cacheRepositorio.Obter<Usuario>($"{USUARIOPORID}-{id}");
        private Usuario ObterUsuarioCachePorCpf(string cpf)
            => cacheRepositorio.Obter<Usuario>($"{USUARIOPORCPF}-{cpf}");
        private Usuario ObterUsuarioCachePorToken(string token)
            => cacheRepositorio.Obter<Usuario>($"{USUARIOPORTOKEN}-{token}");
        private async Task LimparUsuarioCachePorId(long id)
            => await LimparUsuarioCache(ObterUsuarioCachePorId(id));
        private async Task LimparUsuarioCachePorCpf(string cpf)
            => await LimparUsuarioCache(ObterUsuarioCachePorCpf(cpf));
        private async Task LimparUsuarioCachePorToken(string token)
            => await LimparUsuarioCache(ObterUsuarioCachePorToken(token));
        private async Task LimparUsuarioCache(Usuario usuario)
        {
            if (usuario != null)
            {
                try
                {
                    var chaveUsuarioIdCache = $"{USUARIOPORID}-{usuario.Id}";
                    var chaveUsuarioCpfCache = $"{USUARIOPORCPF}-{usuario.Cpf}";
                    var chaveUsuarioTokenCache = $"{USUARIOPORTOKEN}-{usuario.Token}";

                    await Task.WhenAll(
                        cacheRepositorio.RemoverAsync(chaveUsuarioIdCache),
                        cacheRepositorio.RemoverAsync(chaveUsuarioCpfCache),
                        cacheRepositorio.RemoverAsync(chaveUsuarioTokenCache)
                        );
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    throw ex;
                }
            }
        }
        private async Task SalvarUsuarioCache(Usuario usuario)
        {
            if (usuario != null)
            {
                try
                {
                    var chaveUsuarioIdCache = $"{USUARIOPORID}-{usuario.Id}";
                    var chaveUsuarioCpfCache = $"{USUARIOPORCPF}-{usuario.Cpf}";
                    var chaveUsuarioTokenCache = $"{USUARIOPORTOKEN}-{usuario.Token}";

                    await Task.WhenAll(
                        cacheRepositorio.SalvarAsync(chaveUsuarioIdCache, usuario),
                        cacheRepositorio.SalvarAsync(chaveUsuarioCpfCache, usuario),
                        cacheRepositorio.SalvarAsync(chaveUsuarioTokenCache, usuario)
                        );
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    throw ex;
                }
            }
        }
        public override async Task<Usuario> ObterPorIdAsync(long id)
        {
            var usuario = ObterUsuarioCachePorId(id);
            if (usuario == null)
            {
                usuario = await base.ObterPorIdAsync(id);
            }
            return usuario;
        }
        public override async Task RemoverAsync(long id)
        {
            await LimparUsuarioCachePorId(id);
            await base.RemoverAsync(id);
        }
        public override async Task RemoverAsync(Usuario usuario)
        {
            await LimparUsuarioCache(usuario);
            await base.RemoverAsync(usuario);
        }
        public override async Task<long> SalvarAsync(Usuario usuario)
        {
            if (usuario.Id != 0)
                await LimparUsuarioCache(usuario);
            return await base.SalvarAsync(usuario);
        }
    }
}