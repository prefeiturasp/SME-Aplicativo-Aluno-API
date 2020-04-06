using System.Collections;
using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ExemploRepository : IExemploRepository
    {
        public async Task<IEnumerable<string>> ObterNomesDeExemplos()
        {
            using (var conn = new NpgsqlConnection(ConnectionStrings.ConexaoEol))
            {
                conn.Open();
                var alunos = 
                    await conn.QueryAsync<string>(AutenticacaoConsultas.ObterAlunosDoResponsavel);
                conn.Close();
                return alunos.AsEnumerable<string>();
            }
        }
    }
}
