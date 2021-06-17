using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class FrequenciaAlunoSgpRepositorio : IFrequenciaAlunoSgpRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public FrequenciaAlunoSgpRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<IEnumerable<FrequenciaAlunoSgpDto>> ObterFrequenciaAlunoSgp(int desdeAnoLetivo)
        {
            const string sqlSelect = @"
				select
					t.ano_letivo AnoLetivo,
					ue.ue_id CodigoUe,
					ue.nome NomeUe,
					t.turma_id CodigoTurma,
					t.nome NomeTurma,
					fa.codigo_aluno CodigoAluno,
					fa.bimestre Bimestre,
					coalesce (cc.descricao_sgp, cc.descricao) ComponenteCurricular,
					cc.id  CodigoComponenteCurricular,
					total_aulas QuantidadeAulas,
					total_ausencias QuantidadeAusencias,
					total_compensacoes QuantidadeCompensacoes,
        				(
        					select array_to_string(array(
        					select concat(to_char(a.data_aula, 'YYYY-MM-DD'),':',raa.numero_aula::text)
							from registro_frequencia rf
							inner join aula a on a.id = rf.aula_id
							inner join registro_ausencia_aluno raa on raa.registro_frequencia_id = rf.id 
							where not (a.excluido or raa.excluido)
							and fa.codigo_aluno = raa.codigo_aluno 
							and a.turma_id = t.turma_id 
							and a.tipo_calendario_id = pe.tipo_calendario_id 
							and a.disciplina_id = fa.disciplina_id
							and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
							),',')
						) DiasAusencias
				from frequencia_aluno fa 
				inner join turma t on t.turma_id = fa.turma_id 
				inner join ue on ue.id  = t.ue_id 
				inner join periodo_escolar pe on pe.id = fa.periodo_escolar_id 
				left join componente_curricular cc on cc.id::varchar = fa.disciplina_id
				where
    				not (fa.excluido)
				and cc.permite_registro_frequencia
				and	fa.tipo = 1
				and (t.ano_letivo >= 2020 and t.ano_letivo >= @desdeAnoLetivo)
				union 
				select 
					AnoLetivo,
					CodigoUe,
					NomeUe,
					CodigoTurma,
					NomeTurma,
					'' CodigoAluno,
					Bimestre,
					ComponenteCurricular,
					CodigoComponenteCurricular,
					sum(QuantidadeAulas) QuantidadeAulas,
					0 QuantidadeAusencias,
					0 QuantidadeCompensacoes,
					'' DiasAusencias
				from (
					select 
						t.ano_letivo AnoLetivo,
						ue.ue_id CodigoUe,
						ue.nome NomeUe,
						t.turma_id CodigoTurma,
						t.nome NomeTurma,
						pe.bimestre Bimestre,
						coalesce (cc.descricao_sgp, cc.descricao) ComponenteCurricular,
						cc.id  CodigoComponenteCurricular,
						a.quantidade QuantidadeAulas
					from registro_frequencia rf
					inner join aula a on a.id = rf.aula_id
					inner join periodo_escolar pe on pe.tipo_calendario_id = a.tipo_calendario_id and a.data_aula between pe.periodo_inicio and pe.periodo_fim 
					inner join turma t on t.turma_id = a.turma_id 
					inner join ue on ue.id  = t.ue_id 
					left join componente_curricular cc on cc.id::varchar = a.disciplina_id
					where
						not (rf.excluido or a.excluido)
					and	cc.permite_registro_frequencia
					and (t.ano_letivo >= 2020 and t.ano_letivo >= @desdeAnoLetivo)
					) aaa 
				group by 
					AnoLetivo,
					CodigoUe,
					NomeUe,
					CodigoTurma,
					NomeTurma,
					Bimestre,
					ComponenteCurricular,
					CodigoComponenteCurricular
            ";
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var frequenciaAlunosSgp =
                    await conexao
                    .QueryAsync<FrequenciaAlunoSgpDto>(sqlSelect, new { desdeAnoLetivo });

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