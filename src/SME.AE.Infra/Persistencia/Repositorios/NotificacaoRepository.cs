using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotificacaoRepository : INotificacaoRepository
    {
        public async Task<IEnumerable<Notificacao>> ObterPorGrupo(string grupo)
        {
            IEnumerable<Notificacao> list = null;
            
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    list = await conn.QueryAsync<Notificacao>(NotificacaoConsultas.Select + "WHERE Grupo = @Grupo", new
                    {
                        Grupo = grupo
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return list;
            }

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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }

            return list.FirstOrDefault();
        }

        public async Task<Notificacao> Criar(Notificacao notificacao)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    notificacao.CriadoEm = DateTime.Now;
                    var resultado = await conn.InsertAsync(notificacao);
                    notificacao.Id = resultado;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }

            return notificacao;
        }

        public async Task<Notificacao> Atualizar(Notificacao notificacao)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    var resultado = await conn.UpdateAsync(notificacao);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
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
                    resultado = await conn.DeleteAsync(notificacao);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return resultado;
            }

            return resultado;
        }
    }
}