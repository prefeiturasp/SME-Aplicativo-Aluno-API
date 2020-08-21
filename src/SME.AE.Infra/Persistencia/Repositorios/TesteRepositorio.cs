using Dapper;
using Npgsql;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading.Tasks;

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
