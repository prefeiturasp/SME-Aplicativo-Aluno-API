using Dapper;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Consultas;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AutenticacaoRepositorio : IAutenticacaoRepositorio
    {
        public AutenticacaoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new System.ArgumentNullException(nameof(variaveisGlobaisOptions));
        }

        private readonly string whereReponsavelAluno = @"WHERE responsavel.cd_cpf_responsavel = @cpf AND responsavel.dt_fim IS NULL  AND responsavel.cd_cpf_responsavel IS NOT NULL";
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public async Task<IEnumerable<RetornoUsuarioEol>> SelecionarAlunosResponsavel(string cpf)
        {
            using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
            return (await conexao.QueryAsync<RetornoUsuarioEol>(
                $"{AutenticacaoConsultas.ObterResponsavel} {whereReponsavelAluno}",
                new { cpf }
                )).ToList();
        }
    }
}
