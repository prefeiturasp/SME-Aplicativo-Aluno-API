﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Npgsql;
using Sentry;
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
                    list = await conn.QueryAsync<Notificacao>(
                        NotificacaoConsultas.Select
                        + "WHERE string_to_array(Grupo,',') && string_to_array(@Grupo,',')", new
                        {
                            Grupo = grupo
                        });
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
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
        public async Task<IDictionary<string, object>> ObterGruposDoResponsavel(string cpf, string grupos)
        {
            IDictionary<string, object> list = null;

            try
            {
                await using (var conn = new SqlConnection(ConnectionStrings.ConexaoEol))
                {
                    conn.Open();
                    var query = "select top 1 " + grupos + NotificacaoConsultas.GruposDoResponsavel;
                    var resultado = await conn.QueryAsync(query, new { cpf = cpf });

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

        public async Task<Notificacao> Criar(Notificacao notificacao)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(ConnectionStrings.Conexao))
                {
                    conn.Open();
                    notificacao.CriadoEm = DateTime.Now;
                    await conn.ExecuteAsync(
                        @"INSERT INTO notificacao(id, mensagem, titulo, grupo, dataEnvio, dataExpiracao, criadoEm, criadoPor, alteradoEm, alteradoPor) 
                            VALUES(@Id, @Mensagem, @Titulo, @Grupo, @DataEnvio, @DataExpiracao, @CriadoEm, @CriadoPor, @AlteradoEm,  @AlteradoPor)",
                        notificacao);
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
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