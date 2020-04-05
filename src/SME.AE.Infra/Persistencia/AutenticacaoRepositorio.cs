using Dapper;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia
{
    public class AutenticacaoRepositorio : IAutenticacaoRepositorio
    {
        private readonly string whereReponsavelAluno = @"WHERE responsavel.cd_cpf_responsavel = @cpf " +
            "AND aluno.dt_nascimento_aluno = @dataNascimentoAluno";
        public async Task<RetornoUsuarioEol> SelecionarResponsavel(string cpf, DateTime dataNascimentoAluno)
        {
            using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
            return await conexao.QueryFirstOrDefaultAsync<RetornoUsuarioEol>($"{AutenticacaoConsultas.ObterResponsavel}{whereReponsavelAluno}", new { cpf, dataNascimentoAluno });
        }
    }
}
