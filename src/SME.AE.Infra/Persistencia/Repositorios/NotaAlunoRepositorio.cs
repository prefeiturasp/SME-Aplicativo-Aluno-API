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
    public class NotaAlunoRepositorio : INotaAlunoRepositorio
    {
        private readonly ICacheRepositorio cacheRepositorio;
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.Conexao);

        public NotaAlunoRepositorio(ICacheRepositorio cacheRepositorio)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        private static string chaveCacheAnoUeTurmaAluno(int anoLetivo, string codigoUe, string codigoTurmna, string codigoAluno)
    => $"notasAluno-AnoUeTurmaAluno-{anoLetivo}-{codigoUe}-{codigoTurmna}-{codigoAluno}";

        public async Task SalvarNotaAluno(NotaAlunoSgpDto notaAluno) {
            const string sqlUpdate =
                @"
                update 
	                nota_aluno
                set 
	                componente_curricular=@ComponenteCurricular, 
                    nota=@Nota
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
	                nota
                ) values (
                    @AnoLetivo,
                    @CodigoUe,
                    @CodigoTurma,
                    @Bimestre,
                    @CodigoAluno,
                    @CodigoComponenteCurricular,
                    @ComponenteCurricular,
                    @Nota
                )
                ";

            using var conn = CriaConexao();

            try
            {
                conn.Open();
                var alterado = (await conn.ExecuteAsync(sqlUpdate, notaAluno)) > 0;
                if (!alterado)
                    await conn.ExecuteAsync(sqlInsert, notaAluno);
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
                    .WithDegreeOfParallelism(20)
                    .ForAll(async notaAluno => await SalvarNotaAluno(notaAluno));
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<NotaAlunoResposta>> ObterNotasAluno(int anoLetivo, string codigoUe, string codigoTurma, string codigoAluno)
        {
            try
            {
                var chaveCache = chaveCacheAnoUeTurmaAluno(anoLetivo, codigoUe, codigoTurma, codigoAluno);

                var notasAluno = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(notasAluno))
                    return JsonConvert.DeserializeObject<IEnumerable<NotaAlunoResposta>>(notasAluno);

                using var conexao = CriaConexao();
                conexao.Open();
                var dadosNotasAluno = await conexao.QueryAsync<NotaAlunoResposta>(@"SELECT 
                                                                                                        ano_letivo as AnoLetivo,
                                                                                                        ue_codigo as CodigoUe,
                                                                                                        turma_codigo as CodigoTurma,
	                                                                                                    aluno_codigo as AlunoCodigo,
                                                                                                        bimestre as Bimestre,
                                                                                                        componente_curricular as ComponenteCurricular,
                                                                                                        nota as Nota
                                                                                                    FROM public.nota_aluno 
                                                                                                    WHERE 
                                                                                                        ano_letivo = @anoLetivo
                                                                                                        AND ue_codigo = @CodigoUe 
                                                                                                        AND turma_codigo = @CodigoTurma 
                                                                                                        AND aluno_codigo = @CodigoAluno ", new { anoLetivo, codigoUe, codigoTurma, codigoAluno });
                conexao.Close();

                await cacheRepositorio.SalvarAsync(chaveCache, dadosNotasAluno, 720, false);
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