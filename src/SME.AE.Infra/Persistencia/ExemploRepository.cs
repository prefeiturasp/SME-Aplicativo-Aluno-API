using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia
{
    public class ExemploRepository : IExemploRepository
    {
        public async Task<IEnumerable<string>> ObterNomesDeExemplos()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionStrings.ConexaoEol))
            {
                return await conexao.QueryAsync<string>(AutenticacaoConsultas.ObterResponsavel);
            }
        }
    }
}
