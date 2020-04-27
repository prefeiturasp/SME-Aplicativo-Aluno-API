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
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AlunoRepositorio : IAlunoRepositorio
    {
        private readonly string whereReponsavelAluno = @" WHERE responsavel.cd_cpf_responsavel = @cpf 
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";
        public async Task<IEnumerable<AlunoRespostaEol>> ObterDadosAlunos(string cpf)
        {
            try
            {
                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
                IEnumerable<AlunoRespostaEol> listaAlunos = await conexao.QueryAsync<AlunoRespostaEol>($"{AlunoConsultas.ObterDadosAlunos}{whereReponsavelAluno}", new { cpf });
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