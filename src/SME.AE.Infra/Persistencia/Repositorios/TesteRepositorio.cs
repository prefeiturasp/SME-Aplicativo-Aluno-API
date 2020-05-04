using Dapper;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using Sentry;
using Npgsql;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class TesteRepositorio : ITesteRepositorio
    {
        public async Task<DateTime> ObterDataHoraBanco()
        {
            await using var conn = new NpgsqlConnection(ConnectionStrings.Conexao);
            conn.Open();
            var resultado = await conn.QueryFirstAsync<DateTime>("select now()");
            conn.Close();
            return resultado;
        }
    }
}