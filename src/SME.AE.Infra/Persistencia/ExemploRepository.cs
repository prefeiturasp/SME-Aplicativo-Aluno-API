using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Infra.Persistencia.Consultas;

namespace SME.AE.Infra.Persistencia
{
    public class ExemploRepository : IExemploRepository
    {
        public IEnumerable<string> ObterNomesDeExemplos()
        {
            using (SqlConnection conexao = new SqlConnection(ConnectionStrings.ConexaoEol))
            {
                return conexao.Query<string>(AutenticacaoConsultas.ObterAlunosDoResponsavel);
            }
        }
    }
}
