using Dapper;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia
{
    public class AutenticacaoRepositorio : IAutenticacaoRepositorio
    {
        private readonly string whereReponsavelAluno = "@ WHERE responsalvel.cpf = @cpf " +
            "AND aluno.dt_nascimento_aluno = @dataNascimentoAluno";
        public async Task<bool> ValidarUsuarioEol(string cpf, DateTime dataNascimentoAluno)
        {
            using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
            var aluno = await conexao.QueryFirstAsync<string>($"{AutenticacaoConsultas.ObterAlunosDoResponsavel}{whereReponsavelAluno}", (cpf, dataNascimentoAluno));
            return string.IsNullOrEmpty(aluno);
        }
    }
}
