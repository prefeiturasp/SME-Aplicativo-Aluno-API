using Dapper;
using Dommel;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        public async Task<IEnumerable<NotificacaoPorUsuario>> ObterPorGrupoUsuario(string grupo, string cpf)
        {
            IEnumerable<NotificacaoPorUsuario> list = null;

            var query = NotificacaoConsultas.ObterPorUsuarioLogado
                    //"WHERE UNL.usuario_cpf = @cpf" +
                  +   " WHERE (DATE(DataExpiracao) >= @dataAtual OR DataExpiracao IS NULL) " +
                    " AND (DATE(DataEnvio) <= @dataAtual) ";

          
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                var dataAtual = DateTime.Now.Date;
                list = await conn.QueryAsync<NotificacaoPorUsuario>(
                    query, new
                    {
                        grupo,
                        cpf,
                        dataAtual
                    });
                conn.Close();
            return list;
        }

        public async Task<Notificacao> ObterPorId(long id)
        {
            IEnumerable<Notificacao> list = null;

            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    list = await conn.QueryAsync<Notificacao>(NotificacaoConsultas.Select + "WHERE Id = @id", new
                    {
                        Id = id
                    });
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

            return list.FirstOrDefault();
        }

        // TODO Refatorar para montar a query aqui ao inves de receber por parametro
        public async Task<IDictionary<string, object>> ObterGruposDoResponsavel(string cpf, string grupos, string nomeGrupos)
        {
            IDictionary<string, object> list = null;

            try
            {
                await using (var conn = new SqlConnection(ConnectionStrings.ConexaoEol))
                {
                    conn.Open();
                    var query = $"select {nomeGrupos} from(select {grupos + NotificacaoConsultas.GruposDoResponsavel}) grupos";
                    var resultado = await conn.QueryAsync(query, new { cpf });

                    if (resultado.Any())
                        list = resultado.First() as IDictionary<string, object>;

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

            return list;
        }

        public async Task<IEnumerable<string>> ObterResponsaveisPorGrupo(string where)
        {

            try
            {
                await using (var conn = new SqlConnection(ConnectionStrings.ConexaoEol))
                {
                    conn.Open();
                    var query = $"{NotificacaoConsultas.ResponsaveisPorGrupo}{where}";
                    var resultado = await conn.QueryAsync<string>(query);
                    conn.Close();
                    if (resultado.Any())
                        return resultado;
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
            return null;
        }

        public async Task Criar(Notificacao notificacao)
        {
          
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                notificacao.InserirAuditoria();
                notificacao.InserirCategoria();
                 await conn.InsertAsync(notificacao);
                conn.Close();
        }


        public async Task InserirNotificacaoAluno(NotificacaoAluno notificacaoAluno)
        {
                await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
                conn.Open();
                notificacaoAluno.InserirAuditoria();
                await conn.InsertAsync(notificacaoAluno);
                conn.Close();
        }

        public async Task InserirNotificacaoTurma(NotificacaoTurma notificacaoTurma)
        {
             await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
             conn.Open();
             notificacaoTurma.InserirAuditoria();
             await conn.InsertAsync(notificacaoTurma);
             conn.Close();
        }




        public async Task<Notificacao> Atualizar(Notificacao notificacao)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    await conn.ExecuteAsync(
                        @"UPDATE notificacao set mensagem=@Mensagem, titulo=@Titulo, grupo=@Grupo, 
                                    dataEnvio=@DataEnvio, dataExpiracao=@DataExpiracao, criadoEm=@CriadoEm, 
                                    criadoPor=@CriadoPor, alteradoEm=@AlteradoEm, alteradoPor=@AlteradoPor 
                               WHERE id=@Id",
                        notificacao);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return null;
            }

            return notificacao;
        }

        public async Task<bool> Remover(Notificacao notificacao)
        {
            bool resultado = false;

            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();

                    var retorno = await conn.ExecuteAsync(
                        @"DELETE FROM notificacao where id = @ID", notificacao);
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