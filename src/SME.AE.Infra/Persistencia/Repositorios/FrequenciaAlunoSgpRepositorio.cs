using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class FrequenciaAlunoSgpRepositorio : IFrequenciaAlunoSgpRepositorio
    {
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);

        public async Task<IEnumerable<FrequenciaAlunoSgpDto>> ObterFrequenciaAlunoSgp(int desdeAnoLetivo)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var frequenciaAlunosSgp = await conexao
                    .QueryAsync<FrequenciaAlunoSgpDto>(
                        @"
                        select 
	                        t.ano_letivo AnoLetivo,
	                        ue.ue_id CodigoUe,
	                        ue.nome NomeUe,
	                        t.turma_id::int8 CodigoTurma,
	                        t.nome NomeTurma,
	                        fa.codigo_aluno CodigoAluno,
	                        fa.bimestre Bimestre,
	                        coalesce (cc.descricao_sgp, cc.descricao) ComponenteCurricular,
	                        total_aulas QuantidadeAulas,
	                        total_ausencias QuantidadeAusencias,
	                        total_compensacoes QuantidadeCompensacoes
                        from frequencia_aluno fa 
                        inner join turma t on t.turma_id = fa.turma_id 
                        inner join ue on ue.id  = t.ue_id 
                        left join componente_curricular cc on cc.id::varchar = fa.disciplina_id 
                        where 
	                        cc.permite_registro_frequencia
	                        and (t.ano_letivo >= 2020 and t.ano_letivo >= @desdeAnoLetivo)
                        order by 
	                        ue.ue_id, t.turma_id, t.ano_letivo, fa.bimestre, fa.codigo_aluno 	
                        ", new { desdeAnoLetivo });
                conexao.Close();

                return frequenciaAlunosSgp;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}