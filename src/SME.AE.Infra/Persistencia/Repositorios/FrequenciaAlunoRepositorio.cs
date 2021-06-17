using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.FrequenciasDoAluno.PorComponenteCurricular;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class FrequenciaAlunoRepositorio : IFrequenciaAlunoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public FrequenciaAlunoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task<FrequenciaAlunoPorComponenteCurricularResposta> ObterFrequenciaAlunoPorComponenteCurricularAsync(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno, short codigoComponenteCurricular)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var query = @"SELECT
                            ano_letivo as AnoLetivo,
                            ue_codigo as CodigoUe,
                            ue_nome as NomeUe,
                            turma_codigo as CodigoTurma,
                            turma_descricao as NomeTurma,
                            aluno_codigo as AlunoCodigo,
                            componente_curricular_codigo AS CodigoComponenteCurricular,
                            componente_curricular as ComponenteCurricular,
                            '-' AS splitOn,
                            bimestre as Bimestre,
                            quantidade_aulas as QuantidadeAulas,
                            quantidade_faltas as QuantidadeFaltas,
                            quantidade_compensacoes as QuantidadeCompensacoes,
                            dias_ausencias AS DiasAusencia
                        FROM public.frequencia_aluno
                        WHERE
                            ano_letivo = @anoLetivo
                            AND ue_codigo = @codigoUe
                            AND turma_codigo = @codigoTurma
                            AND aluno_codigo = @codigoAluno
                            AND componente_curricular_codigo = @codigoComponenteCurricular
                        UNION 
                        SELECT
                            ano_letivo as AnoLetivo,
                            ue_codigo as CodigoUe,
                            ue_nome as NomeUe,
                            turma_codigo as CodigoTurma,
                            turma_descricao as NomeTurma,
                            @codigoAluno as AlunoCodigo,
                            componente_curricular_codigo AS CodigoComponenteCurricular,
                            componente_curricular as ComponenteCurricular,
                            '-' AS splitOn,
                            bimestre as Bimestre,
                            quantidade_aulas as QuantidadeAulas,
                            quantidade_faltas as QuantidadeFaltas,
                            quantidade_compensacoes as QuantidadeCompensacoes,
                            dias_ausencias AS DiasAusencia
                        FROM public.frequencia_aluno a
                        WHERE
                            ano_letivo = @anoLetivo
                            AND ue_codigo = @codigoUe
                            AND turma_codigo = @codigoTurma
                            AND aluno_codigo = ''
                            AND componente_curricular_codigo = @codigoComponenteCurricular
                            and not exists (select 1 FROM public.frequencia_aluno b WHERE a.ano_letivo = b.ano_letivo
    				                        AND a.ue_codigo = b.ue_codigo 
    				                        AND a.turma_codigo = b.turma_codigo
    				                        and a.bimestre = b.bimestre 
   					                        AND aluno_codigo = @codigoAluno
    				                        AND a.componente_curricular_codigo = b.componente_curricular_codigo)";

                var parametros = new { anoLetivo, codigoUe, codigoTurma, codigoAluno, codigoComponenteCurricular };

                var dadosFrequenciaAlunos = await conexao.QueryParentChildSingleAsync<FrequenciaAlunoPorComponenteCurricularResposta, FrequenciaAlunoPorBimestre, short>(
                    query, x => x.CodigoComponenteCurricular, x => x.FrequenciasPorBimestre, parametros, splitOn: "splitOn");
                conexao.Close();

                return dadosFrequenciaAlunos;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<FrequenciaAlunoResposta> ObterFrequenciaAlunoAsync(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var query = @"DROP TABLE IF EXISTS tmp_frequencia_aluno_global;

                            select 
                                AnoLetivo,
                                CodigoUe,
                                CodigoTurma,
                                AlunoCodigo,
                                SUM(QuantidadeAulas) as QuantidadeAulas,
                                SUM(QuantidadeFaltas) as QuantidadeFaltas,
                                SUM(QuantidadeCompensacoes) as QuantidadeCompensacoes
                                INTO temp tmp_frequencia_aluno_global
                            from 
                            (SELECT
                                ano_letivo as AnoLetivo,
                                ue_codigo as CodigoUe,
                                turma_codigo as CodigoTurma,
                                aluno_codigo as AlunoCodigo,
                                SUM(quantidade_aulas) as QuantidadeAulas,
                                SUM(quantidade_faltas) as QuantidadeFaltas,
                                SUM(quantidade_compensacoes) as QuantidadeCompensacoes
                            FROM public.frequencia_aluno
                            WHERE
                                ano_letivo = @anoLetivo
                                AND ue_codigo = @codigoUe
                                AND turma_codigo = @codigoTurma
   	                            AND aluno_codigo = @codigoAluno
                            GROUP BY
                                ano_letivo,
                                ue_codigo,
                                turma_codigo,
                                aluno_codigo
                             union
                            SELECT
                                ano_letivo as AnoLetivo,
                                ue_codigo as CodigoUe,
                                turma_codigo as CodigoTurma,
                                @codigoAluno as AlunoCodigo,
                                SUM(quantidade_aulas) as QuantidadeAulas,
                                SUM(quantidade_faltas) as QuantidadeFaltas,
                                SUM(quantidade_compensacoes) as QuantidadeCompensacoes
                            FROM public.frequencia_aluno a
                            WHERE
                                ano_letivo = @anoLetivo
                                AND ue_codigo = @codigoUe
                                AND turma_codigo = @codigoTurma
   	                            AND aluno_codigo = ''
	                            and not exists (select 1 FROM public.frequencia_aluno b WHERE a.ano_letivo = b.ano_letivo
    				                            AND a.ue_codigo = b.ue_codigo 
    				                            AND a.turma_codigo = b.turma_codigo
    				                            and a.bimestre = b.bimestre 
   					                            AND aluno_codigo = @codigoAluno)   	   	
                            GROUP BY
                                ano_letivo,
                                ue_codigo,
                                turma_codigo,
                                aluno_codigo) aaa
                            GROUP BY
                                AnoLetivo,
                                CodigoUe,
                                CodigoTurma,
                                AlunoCodigo;

                            DROP TABLE IF EXISTS tmp_frequencia_aluno_componentes_curriculares;
                            SELECT
	                            distinct
                                ano_letivo as AnoLetivo,
                                ue_codigo as CodigoUe,
                                turma_codigo as CodigoTurma,
                                aluno_codigo as AlunoCodigo,
                                componente_curricular_codigo as CodigoComponenteCurricular,
                                componente_curricular as DescricaoComponenteCurricular
                            INTO tmp_frequencia_aluno_componentes_curriculares
                            FROM  public.frequencia_aluno
                            WHERE
	                            ano_letivo = @anoLetivo
                                AND ue_codigo = @codigoUe
                                AND turma_codigo = @codigoTurma
                                AND aluno_codigo = @codigoAluno
                            ORDER BY componente_curricular;

                            SELECT
                                glob.AnoLetivo,
                                glob.CodigoUe,
                                glob.CodigoTurma,
                                glob.AlunoCodigo,
                                glob.QuantidadeAulas,
                                glob.QuantidadeFaltas,
                                glob.QuantidadeCompensacoes,
                                '-' as splitOn,
                                comp.CodigoComponenteCurricular,
                                comp.DescricaoComponenteCurricular
                            FROM tmp_frequencia_aluno_global glob
                            INNER JOIN tmp_frequencia_aluno_componentes_curriculares comp
	                            on glob.AnoLetivo = comp.AnoLetivo
	                            AND glob.CodigoUe = comp.CodigoUe
	                            AND glob.CodigoTurma = comp.CodigoTurma
	                            AND glob.AlunoCodigo = comp.AlunoCodigo;";

                var parametros = new { anoLetivo, codigoUe, codigoTurma, codigoAluno };
                var dadosFrequenciaAluno = await conexao.QueryParentChildSingleAsync<FrequenciaAlunoResposta, ComponenteCurricularDoAluno, string>(query,
                    x => x.AlunoCodigo, x => x.ComponentesCurricularesDoAluno, parametros, splitOn: "splitOn");

                conexao.Close();

                return dadosFrequenciaAluno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task SalvarFrequenciaAluno(FrequenciaAlunoSgpDto frequenciaAluno)
        {
            const string sqlDelete =
                @"
                delete from
	                frequencia_aluno
                where 
	                ue_codigo = @CodigoUe and 
	                turma_codigo = @CodigoTurma and 
                    componente_curricular_codigo = @CodigoComponenteCurricular and
	                ano_letivo = @AnoLetivo and
	                bimestre = @Bimestre and 
	                aluno_codigo = @CodigoAluno
                ";

            const string sqlUpdate =
                @"
                UPDATE
	                frequencia_aluno
                SET
	                ue_nome=@NomeUe,
	                turma_descricao=@NomeTurma,
	                componente_curricular=@ComponenteCurricular,
                    dias_ausencias=@DiasAusencias,
	                quantidade_aulas=@QuantidadeAulas,
	                quantidade_faltas=@QuantidadeAusencias,
	                quantidade_compensacoes=@QuantidadeCompensacoes
                where
	                ano_letivo = @AnoLetivo and
	                bimestre = @Bimestre and
	                ue_codigo = @CodigoUe and
	                turma_codigo = @CodigoTurma and
	                aluno_codigo = @CodigoAluno and
                    componente_curricular_Codigo = @CodigoComponenteCurricular;
                ";

            const string sqlInsert =
                @"
                INSERT INTO frequencia_aluno
                (
	                ue_codigo,
	                ue_nome,
	                turma_codigo,
	                turma_descricao,
	                aluno_codigo,
	                bimestre,
	                componente_curricular_codigo,
	                componente_curricular,
	                quantidade_aulas,
	                quantidade_faltas,
	                quantidade_compensacoes,
                    dias_ausencias,
	                ano_letivo
                ) values (
	                @CodigoUe,
	                @NomeUe,
	                @CodigoTurma,
	                @NomeTurma,
	                @CodigoAluno,
	                @Bimestre,
	                @CodigoComponenteCurricular,
	                @ComponenteCurricular,
	                @QuantidadeAulas,
	                @QuantidadeAusencias,
	                @QuantidadeCompensacoes,
                    @DiasAusencias,
	                @AnoLetivo
                )
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                var alterado = (await conn.ExecuteAsync(sqlUpdate, frequenciaAluno));
                if (alterado == 0)
                {
                    await conn.ExecuteAsync(sqlInsert, frequenciaAluno);
                }
                else if (alterado > 1)
                {
                    await conn.ExecuteAsync(sqlDelete, frequenciaAluno);
                    await conn.ExecuteAsync(sqlInsert, frequenciaAluno);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task SalvarFrequenciaAlunosBatch(IEnumerable<FrequenciaAlunoSgpDto> frequenciaAlunosSgp)
        {
            try
            {
                frequenciaAlunosSgp
                    .AsParallel()
                    .WithDegreeOfParallelism(4)
                    .ForAll(async frequenciaAluno => await SalvarFrequenciaAluno(frequenciaAluno));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
            await Task.CompletedTask;
        }

        public async Task ExcluirFrequenciaAluno(FrequenciaAlunoSgpDto frequenciaAluno)
        {
            const string sqlDelete =
                @"
                delete from
	                frequencia_aluno
                where
	                ue_codigo = @CodigoUe and
	                turma_codigo = @CodigoTurma and
                    componente_curricular_codigo = @CodigoComponenteCurricular and
	                ano_letivo = @AnoLetivo and
	                bimestre = @Bimestre and
	                aluno_codigo = @CodigoAluno
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                await conn.ExecuteAsync(sqlDelete, frequenciaAluno);
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<FrequenciaAlunoSgpDto>> ObterListaParaExclusao(int desdeAnoLetivo)
        {
            const string sqlSelect =
                @"
                select
                    ue_codigo CodigoUe,
                    ue_nome NomeUe,
                    turma_codigo CodigoTurma,
                    turma_descricao NomeTurma,
                    aluno_codigo CodigoAluno,
                    bimestre Bimestre,
                    componente_curricular_codigo CodigoComponenteCurricular,
                    componente_curricular ComponenteCurricular,
                    quantidade_aulas QuantidadeAulas,
                    quantidade_faltas QuantidadeAusencias,
                    quantidade_compensacoes QuantidadeCompensacoes,
                    dias_ausencias DiasAusencias,
                    ano_letivo AnoLetivo
                from
                    frequencia_aluno
                where ano_letivo >= @desdeAnoLetivo
                ";

            try
            {
                using var conn = CriaConexao();
                conn.Open();
                var frequenciaAlunoLista = await conn.QueryAsync<FrequenciaAlunoSgpDto>(sqlSelect, new { desdeAnoLetivo });
                conn.Close();
                return frequenciaAlunoLista;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}