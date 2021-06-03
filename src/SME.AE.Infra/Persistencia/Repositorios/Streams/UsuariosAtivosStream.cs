using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Sentry;
using Sentry.Protocol;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Repositorios.Streams
{
    public class UsuariosAtivosStream : IAsyncEnumerable<Usuario>, IDisposable
    {
        private NpgsqlConnection _connection;

        private readonly String MENSAGEM_ERRO_AO_CRIAR_STREAM =
            "Não foi possível criar stream para os usuários ativos no banco";

        private readonly String CONSULTA = "select * from usuario where excluido = false";
        
        public UsuariosAtivosStream()
        {
            _connection = new NpgsqlConnection(ConnectionStrings.Conexao);
            _connection.Open();
        }

        public async IAsyncEnumerator<Usuario> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerator<Usuario> enumeradorUsuarios = null;

            try
            {
                var usuarios = await AbreConexaoEConsulta();
                enumeradorUsuarios = usuarios.GetEnumerator();
            }
            catch (Exception ex)
            {
                await DisposeAsync();
                SentrySdk.CaptureMessage(
                    String.Format("[ UsuariosAtivosStream ] Erro ao abrir conexão ou obter numerador: {}", ex.Message),
                    SentryLevel.Error);
            }

            if (enumeradorUsuarios == null)
            {
                SentrySdk.CaptureMessage(MENSAGEM_ERRO_AO_CRIAR_STREAM);
                throw new Exception(MENSAGEM_ERRO_AO_CRIAR_STREAM);
            }

            while (enumeradorUsuarios.MoveNext())
            {
                yield return enumeradorUsuarios.Current;
            }

            await DisposeAsync();
        }

        private async Task<IEnumerable<Usuario>> AbreConexaoEConsulta()
        {
            using var conexao = new NpgsqlConnection(ConnectionStrings.Conexao);
            await conexao.OpenAsync();
            return await conexao.QueryAsync<Usuario>(CONSULTA);
        }

        public async Task DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
            }
        }

        public void Dispose()
        {
            Task.WaitAll(DisposeAsync());
        }
    }
}