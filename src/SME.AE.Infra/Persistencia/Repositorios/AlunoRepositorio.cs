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

namespace SME.AE.Infra.Persistencia.Repositorios
{
   public class AlunoRepositorio : IAlunoRepositorio
    {
        public async Task<IEnumerable<Aluno>> ObterDadosAlunos(string cpf)
        {
            using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
            return await conexao.QueryAsync<Aluno>(AlunoConsultas.ObterDadosAlunos);
        }
    }
}