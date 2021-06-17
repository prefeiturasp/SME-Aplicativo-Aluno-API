using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotaAlunoRepositorio : INotaAlunoRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public NotaAlunoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.AEConnection);

        public async Task SalvarNotaAluno(NotaAlunoSgpDto notaAluno)
        {
            const string sqlDelete =
                @"
                delete from
	                nota_aluno
                where 
	                ano_letivo = @AnoLetivo
                and ue_codigo = @CodigoUe
                and turma_codigo = @CodigoTurma
                and bimestre = @Bimestre
                and aluno_codigo = @CodigoAluno
                and componente_curricular_codigo = @CodigoComponenteCurricular
                ";

            const string sqlUpdate =
                @"
                update 
	                nota_aluno
                set 
	                componente_curricular=@ComponenteCurricular, 
                    nota=@Nota,
                    nota_descricao=@NotaDescricao,
                    recomendacoes_aluno=@RecomendacoesAluno,
                    recomendacoes_familia=@RecomendacoesFamilia
                where 
	                ano_letivo = @AnoLetivo
                and ue_codigo = @CodigoUe
                and turma_codigo = @CodigoTurma
                and bimestre = @Bimestre
                and aluno_codigo = @CodigoAluno
                and componente_curricular_codigo = @CodigoComponenteCurricular
                ";

            const string sqlInsert =
                @"
                insert into nota_aluno (
	                ano_letivo, 
	                ue_codigo, 
	                turma_codigo, 
	                bimestre, 
	                aluno_codigo, 
	                componente_curricular_codigo, 
	                componente_curricular, 
	                nota,
                    nota_descricao,
                    recomendacoes_aluno,
                    recomendacoes_familia
                ) values (
                    @AnoLetivo,
                    @CodigoUe,
                    @CodigoTurma,
                    @Bimestre,
                    @CodigoAluno,
                    @CodigoComponenteCurricular,
                    @ComponenteCurricular,
                    @Nota,
                    @NotaDescricao,
                    @RecomendacoesAluno,
                    @RecomendacoesFamilia
                )
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                var alterado = (await conn.ExecuteAsync(sqlUpdate, notaAluno));
                if (alterado == 0)
                {
                    await conn.ExecuteAsync(sqlInsert, notaAluno);
                }
                else if (alterado > 1)
                {
                    await conn.ExecuteAsync(sqlDelete, notaAluno);
                    await conn.ExecuteAsync(sqlInsert, notaAluno);
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
        public async Task ExcluirNotaAluno(NotaAlunoSgpDto notaAluno)
        {
            const string sqlDelete =
                @"
                delete from
	                nota_aluno
                where 
	                ano_letivo = @AnoLetivo
                and ue_codigo = @CodigoUe
                and turma_codigo = @CodigoTurma
                and bimestre = @Bimestre
                and aluno_codigo = @CodigoAluno
                and componente_curricular_codigo = @CodigoComponenteCurricular
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                await conn.ExecuteAsync(sqlDelete, notaAluno);
                conn.Close();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
        public async Task SalvarNotaAlunosBatch(IEnumerable<NotaAlunoSgpDto> notaAlunosSgp)
        {
            try
            {
                notaAlunosSgp
                    .AsParallel()
                    .WithDegreeOfParallelism(4)
                    .ForAll(async notaAluno => await SalvarNotaAluno(notaAluno));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
            await Task.CompletedTask;
        }
        public async Task<IEnumerable<NotaAlunoSgpDto>> ObterListaParaExclusao(int desdeAnoLetivo)
        {
            const string sqlSelect =
                @"
                Select
	                ano_letivo                      AnoLetivo,
	                ue_codigo                       CodigoUe,
	                turma_codigo                    CodigoTurma,
	                bimestre                        Bimestre,
	                aluno_codigo                    CodigoAluno,
	                componente_curricular_codigo    CodigoComponenteCurricular,
	                componente_curricular           ComponenteCurricular,
	                nota                            Nota,
                    nota_descricao                  NotaDescricao,
                    recomendacoes_aluno             RecomendacoesAluno,
                    recomendacoes_familia           RecomendacoesFamilia
                from
                    nota_aluno
                where
                    ano_letivo >= @anoLetivo
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                var notaAlunoLista = await conn.QueryAsync<NotaAlunoSgpDto>(sqlSelect, new { anoLetivo = desdeAnoLetivo });
                conn.Close();
                return notaAlunoLista.ToArray();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }


        public async Task<NotaAlunoPorBimestreResposta> ObterNotasAluno(int anoLetivo, short bimestre, string codigoUe, string codigoTurma, string codigoAluno)
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var query = @"SELECT 
                                distinct
                                ano_letivo as AnoLetivo,
                                ue_codigo as CodigoUe,
                                turma_codigo as CodigoTurma,
	                            aluno_codigo as AlunoCodigo,
                                bimestre as Bimestre,
                                recomendacoes_familia as RecomendacoesFamilia,
                                recomendacoes_aluno as RecomendacoesAluno,
                                '-' AS splitOn,
                                componente_curricular as ComponenteCurricular,
                                nota as Nota,
                                nota_descricao AS NotaDescricao
                            FROM public.nota_aluno 
                            WHERE 
                                ano_letivo = @anoLetivo
                                AND bimestre = @bimestre
                                AND ue_codigo = @CodigoUe 
                                AND turma_codigo = @CodigoTurma 
                                AND aluno_codigo = @CodigoAluno ";

                var parametros = new { anoLetivo, bimestre, codigoUe, codigoTurma, codigoAluno };
                var dadosNotasAluno = await conexao.QueryParentChildSingleAsync<NotaAlunoPorBimestreResposta, NotaAlunoComponenteCurricular, int>(query,
                    notaAlunoRespostaBimestre => notaAlunoRespostaBimestre.Bimestre,
                    notaAlunoRespostaBimestre => notaAlunoRespostaBimestre.NotasPorComponenteCurricular,
                    parametros, splitOn: "splitOn");
                conexao.Close();

                return dadosNotasAluno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}