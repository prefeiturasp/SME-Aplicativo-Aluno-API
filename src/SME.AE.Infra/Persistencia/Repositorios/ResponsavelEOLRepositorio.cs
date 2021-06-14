using Dapper;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ResponsavelEOLRepositorio : IResponsavelEOLRepositorio
    {
        private SqlConnection CriaConexao() => new SqlConnection(ConnectionStrings.ConexaoEol);
        public async Task<IEnumerable<ResponsavelEOLDto>> ListarCpfResponsavelDaDreUeTurma(long dreCodigo, int anoLetivo)
        {
            var sql =
                @"
					select 
						distinct
						dre.cd_unidade_educacao CodigoDre,
						dre.nm_exibicao_unidade Dre,
						vue.cd_unidade_educacao CodigoUe,
						vue.nm_exibicao_unidade Ue,
						te.cd_turma_escola CodigoTurma,
						coalesce(ra.cd_cpf_responsavel, 0) CpfResponsavel
					from v_aluno_cotic(nolock) a
					inner join responsavel_aluno(nolock) ra on ra.cd_aluno = a.cd_aluno 
					inner join v_matricula_cotic(nolock) m on m.cd_aluno = a.cd_aluno 
					inner join matricula_turma_escola(nolock) mte on mte.cd_matricula = m.cd_matricula 
					inner join v_cadastro_unidade_educacao(nolock) vue on vue.cd_unidade_educacao = m.cd_escola
					inner join v_cadastro_unidade_educacao(nolock) dre on dre.cd_unidade_educacao = vue.cd_unidade_administrativa_referencia
					inner join turma_escola(nolock) te on te.cd_turma_escola = mte.cd_turma_escola
					where 
						mte.cd_situacao_aluno IN ( 1, 6, 10, 13 ) and
						ra.dt_fim IS NULL and dre.cd_unidade_educacao = @dreCodigo
                        and m.an_letivo = @anoLetivo;
				";

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var timer = Stopwatch.StartNew();
            try
            {
                var responsaveisEOL = await conn.QueryAsync<ResponsavelEOLDto>(sql, new { dreCodigo, anoLetivo });
                timer.Stop();
                await conn.CloseAsync();
                return responsaveisEOL;
            }
            catch (Exception)
            {
                timer.Stop();
                Sentry.SentrySdk.CaptureMessage($"Erro ao listar Responsaveis: {sql} - DreCodigo: {dreCodigo} - AnoLetivo: {anoLetivo} - Tempo de execução: {timer.Elapsed}");
                return null;
            }
        }
        public async Task<IEnumerable<ResponsavelAlunoEOLDto>> ListarCpfResponsavelAlunoDaDreUeTurma()
        {
            var sql =
                @"
	            SELECT DISTINCT
                       aluno.cd_aluno                        CodigoAluno, 
		               responsavel.cd_cpf_responsavel        CpfResponsavel,
                       vue.cd_unidade_educacao               CodigoUe,
                       dre.cd_unidade_educacao               CodigoDre,
                       te.cd_turma_escola                    CodigoTurma,
                       te.dc_turma_escola					 Turma,
                       tesc.tp_escola                        CodigoTipoEscola, 
                       etapa_ensino.cd_etapa_ensino 		 CodigoEtapaEnsino,
                       ciclo_ensino.cd_ciclo_ensino 		 CodigoCicloEnsino,
                       serie_ensino.sg_resumida_serie 		 SerieResumida,
                       etapa_ensino.cd_etapa_ensino          CodigoModalidadeTurma
                FROM   v_aluno_cotic aluno 
                INNER JOIN responsavel_aluno responsavel 
                       ON aluno.cd_aluno = responsavel.cd_aluno 
                INNER JOIN(SELECT cd_aluno, 
                                 cd_matricula, 
                                 cd_escola, 
                                 cd_serie_ensino 
                          FROM   v_matricula_cotic 
                          WHERE  st_matricula = 1 
                                 AND cd_serie_ensino IS NOT NULL -- turma regular 
                                 AND an_letivo = Year(Getdate())) AS matricula 
                       ON matricula.cd_aluno = aluno.cd_aluno 
                INNER JOIN(SELECT cd_matricula, 
                                 cd_turma_escola, 
                                 cd_situacao_aluno, 
                                 dt_situacao_aluno, 
                                 CASE 
                                   WHEN cd_situacao_aluno = 1 THEN 'Ativo' 
                           WHEN cd_situacao_aluno = 6 THEN 
                           'Pendente de Rematrícula' 
                           WHEN cd_situacao_aluno = 10 THEN 'Rematriculado' 
                           WHEN cd_situacao_aluno = 13 THEN 'Sem continuidade' 
                           ELSE 'Fora do domínio liberado pela PRODAM' 
                                 END SituacaoMatricula 
                          FROM   matricula_turma_escola (nolock)) mte 
                       ON mte.cd_matricula = matricula.cd_matricula 
                          AND mte.cd_situacao_aluno IN ( 1, 6, 10, 13 ) 
                INNER JOIN v_cadastro_unidade_educacao(nolock) vue 
                       ON vue.cd_unidade_educacao = matricula.cd_escola 
                INNER JOIN escola esc (nolock)
                       ON esc.cd_escola = vue.cd_unidade_educacao 
                INNER JOIN tipo_escola(nolock) tesc 
                       ON tesc.tp_escola = esc.tp_escola 
                          AND tesc.tp_escola IN ( 1, 2, 3, 4, 
                                                  10, 11, 12, 13, 
                                                  14, 15, 16, 17, 
                                                  18, 19, 20, 22, 
                                                  23, 24, 25, 26, 
                                                  27, 28, 29, 30, 31 ) 
                INNER JOIN v_cadastro_unidade_educacao(nolock) dre 
                       ON dre.cd_unidade_educacao = 
                          vue.cd_unidade_administrativa_referencia 
                INNER JOIN turma_escola(nolock) te 
                       ON te.cd_turma_escola = mte.cd_turma_escola 
                INNER JOIN serie_ensino(nolock) 
                       ON matricula.cd_serie_ensino = serie_ensino.cd_serie_ensino 
                INNER JOIN etapa_ensino(nolock)
                       ON serie_ensino.cd_etapa_ensino = etapa_ensino.cd_etapa_ensino 
                INNER JOIN ciclo_ensino (nolock)
                       ON serie_ensino.cd_ciclo_ensino = ciclo_ensino.cd_ciclo_ensino  
                where responsavel.dt_fim is null
               				";

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var responsaveisEOL = await conn.QueryAsync<ResponsavelAlunoEOLDto>(sql);
            await conn.CloseAsync();
            return responsaveisEOL;
        }

        public async Task<ResponsavelAlunoEolResumidoDto> ObterDadosResumidosReponsavelPorCpf(string cpfResponsavel)
        {
            var sql =
                 @"
	            SELECT 
                    RTRIM(LTRIM(responsavel.nm_responsavel)) AS Nome,
                    RTRIM(LTRIM(responsavel.email_responsavel)) AS Email,
                    RTRIM(LTRIM(cd_ddd_celular_responsavel)) AS DDD,
                    RTRIM(LTRIM(nr_celular_responsavel)) AS Celular
                FROM responsavel_aluno responsavel
                WHERE responsavel.cd_cpf_responsavel = @cpfResponsavel 
                  AND responsavel.dt_fim IS NULL  
                  AND responsavel.cd_cpf_responsavel IS NOT NULL";

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var responsavelEol = await conn.QueryFirstOrDefaultAsync<ResponsavelAlunoEolResumidoDto>(sql, new { cpfResponsavel });
            await conn.CloseAsync();
            return responsavelEol;
        }
    }
}
