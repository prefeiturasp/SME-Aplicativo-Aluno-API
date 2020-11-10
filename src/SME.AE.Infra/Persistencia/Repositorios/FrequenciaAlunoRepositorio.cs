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
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class FrequenciaAlunoRepositorio : IFrequenciaAlunoRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);
        private static string chaveCacheAnoUeTurmaAluno(int anoLetivo, string codigoUe, long codigoTurmna, string codigoAluno)
            => $"frequenciaAluno-AnoUeTurmaAluno-{anoLetivo}-{codigoUe}-{codigoTurmna}-{codigoAluno}";

        public FrequenciaAlunoRepositorio(ICacheRepositorio cacheRepositorio) 
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        public async Task<IEnumerable<FrequenciaAlunoResposta>> ObterFrequenciaAluno(int anoLetivo, string codigoUe, long codigoTurma, string codigoAluno)
        {
            try
            {
                var chaveCache = chaveCacheAnoUeTurmaAluno(anoLetivo, codigoUe, codigoTurma, codigoAluno);

                var frequenciaAluno = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(frequenciaAluno))
                    return JsonConvert.DeserializeObject<IEnumerable<FrequenciaAlunoResposta>>(frequenciaAluno);

                using var conexao = CriaConexao();
                conexao.Open();
                var dadosFrequenciaAlunos = await conexao.QueryAsync<FrequenciaAlunoResposta>(@"SELECT 
                                                                                                        ano_letivo as AnoLetivo,
                                                                                                        ue_codigo as CodigoUe,
                                                                                                        ue_nome as NomeUe,
                                                                                                        turma_codigo as CodigoTurma,
                                                                                                        turma_descricao as NomeTurma,
	                                                                                                    aluno_codigo as AlunoCodigo,
                                                                                                        bimestre as Bimestre,
                                                                                                        componente_curricular as ComponenteCurricular,
                                                                                                        quantidade_aulas as QuantidadeAulas,
                                                                                                        quantidade_faltas as QuantidadeFaltas,
                                                                                                        quantidade_compensacoes as QuantidadeCompensacoes
                                                                                                    FROM public.frequencia_aluno 
                                                                                                    WHERE 
                                                                                                        ano_letivo = @anoLetivo
                                                                                                        AND ue_codigo = @CodigoUe 
                                                                                                        AND turma_codigo = @CodigoTurma 
                                                                                                        AND aluno_codigo = @CodigoAluno ", new { anoLetivo, codigoUe, codigoTurma, codigoAluno });
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, dadosFrequenciaAlunos, 720, false);
                return dadosFrequenciaAlunos;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task SalvarFrequenciaAluno(FrequenciaAlunoSgpDto frequenciaAluno) {
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
                var alterado = (await conn.ExecuteAsync(sqlUpdate, frequenciaAluno)) > 0;
                if (!alterado)
                    await conn.ExecuteAsync(sqlInsert, frequenciaAluno);
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