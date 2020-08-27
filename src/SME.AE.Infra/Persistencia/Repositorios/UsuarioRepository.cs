﻿
using Dapper;
using Dapper.Dommel;
using Dommel;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
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

        private readonly ICacheRepositorio cacheRepositorio;

        public UsuarioRepository(ICacheRepositorio cacheRepositorio) : base(ConnectionStrings.Conexao)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<Usuario> ObterPorCpf(string cpf)
        {
            var chaveCache = $"Usuario-{cpf}";
            var usuario = await cacheRepositorio.ObterAsync(chaveCache);
            if (!string.IsNullOrWhiteSpace(usuario))
                return JsonConvert.DeserializeObject<Usuario>(usuario);

            using var conexao = InstanciarConexao();
            conexao.Open();
            var retorno = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Cpf == cpf);
            conexao.Close();

            await cacheRepositorio.SalvarAsync(chaveCache, retorno, 1080, false);
            return retorno;
        }

        public async Task<IEnumerable<string>> ObterTodos()
        {
            try
            {
                var chaveCache = $"Cpfs";
                var cpfs = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(cpfs))
                    return JsonConvert.DeserializeObject<IEnumerable<string>>(cpfs);

                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var retorno = await conn.QueryAsync<string>(UsuarioConsultas.ObterTodos);
                conn.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, retorno, 1080, false);
                return retorno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }
        }

        public async Task AtualizaUltimoLoginUsuario(string cpf)
        {
            var dataHoraAtual = DateTime.Now;
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            await conn.ExecuteAsync(
                "update usuario set ultimologin = @dataHoraAtual, excluido = false  where cpf = @cpf", new { cpf, dataHoraAtual });
            conn.Close();

            var chaveCache = $"Usuario-{cpf}";
            await cacheRepositorio.RemoverAsync(chaveCache);
        }

        public async Task AltualizarUltimoAcessoPrimeiroUsuario(Usuario usuario)
        {
            var sql = @"UPDATE usuario
                SET ultimologin=@ultimologin, excluido=@Excluido, primeiroacesso=@PrimeiroAcesso, 
                alteradoem=@AlteradoEm, alteradopor=@AlteradoPor, token_redefinicao = '', redefinicao = false, validade_token = null
                where id=@Id;";

            using var conexao = InstanciarConexao();
            conexao.Open();
            await conexao.ExecuteAsync(sql, usuario);
            conexao.Close();

            var chaveCache = $"Usuario-{usuario.Cpf}";
            await cacheRepositorio.RemoverAsync(chaveCache);
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

            using var conexao = InstanciarConexao();
            await conexao.ExecuteAsync(builder.ToString(), new { id, email, celular, alteradoem = DateTime.Now });
            conexao.Close();
        }

        public async Task AtualizarPrimeiroAcesso(long id, bool primeiroAcesso)
        {
            var conexao = InstanciarConexao();
            await conexao.ExecuteAsync(@"UPDATE usuario
                            SET primeiroacesso=@primeiroAcesso,alteradopor='Sistema', alteradoem=@alteradoem
                            where id = @id;", new { id, primeiroAcesso, alteradoem = DateTime.Now });
            conexao.Close();
        }

        public async Task<Usuario> ObterUsuarioPorTokenAutenticacao(string token)
        {
            var chaveCache = $"UsuarioToken-{token}";
            var usuarioToken = await cacheRepositorio.ObterAsync(chaveCache);
            if (!string.IsNullOrWhiteSpace(usuarioToken))
                return JsonConvert.DeserializeObject<Usuario>(usuarioToken);

            using var conexao = InstanciarConexao();
            conexao.Open();
            var retorno = await conexao.FirstOrDefaultAsync<Usuario>(x => !x.Excluido && x.Token == token && x.RedefinirSenha);
            conexao.Close();

            await cacheRepositorio.SalvarAsync(chaveCache, retorno, 1080, false); ;
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

            var chaveCache = $"Usuario-{cpf}";
            await cacheRepositorio.RemoverAsync(chaveCache);
        }

        public async Task CriaUsuarioDispositivo(long usuarioId, string dispositivoId)
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
            await cacheRepositorio.SalvarAsync(chaveCache, JsonConvert.SerializeObject(new UsuarioDispositivoDto(usuarioId, dispositivoId)), 1080, false);
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
                if (retorno.Count() == 0)
                    return false;
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