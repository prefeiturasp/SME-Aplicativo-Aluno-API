using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioNotificacaoRepositorio : IUsuarioNotificacaoRepositorio
    {
        public Task<UsuarioNotificacao> Atualizar(Notificacao notificacao)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioNotificacao> Criar(Notificacao notificacao)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioNotificacao> ObterPorId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Remover(long notificacaoId)
        {
            bool resultado = false;

            try
            {
                await using (var conn = new Npgsql.NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();

                    var retorno = await conn.ExecuteAsync(
                        @"DELETE FROM  where id = @ID", );
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return resultado;
            }

            return resultado;
        }
    }
}
