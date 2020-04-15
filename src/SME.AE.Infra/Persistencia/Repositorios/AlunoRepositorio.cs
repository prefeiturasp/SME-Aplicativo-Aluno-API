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

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AlunoRepositorio : IAlunoRepositorio
    {
        private readonly string whereReponsavelAluno = @"WHERE responsavel.cd_cpf_responsavel = @cpf AND responsavel.dt_fim IS NULL  AND responsavel.cd_cpf_responsavel IS NOT NULL";
        public async Task<IEnumerable<Aluno>> ObterDadosAlunos(string cpf)
        {
            try
            {
                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                IEnumerable<Aluno> listaAlunos = await conexao.QueryAsync<Aluno>($"{AlunoConsultas.ObterDadosAlunos}{whereReponsavelAluno}", new { cpf });
                return listaAlunos;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

        }
    }
}