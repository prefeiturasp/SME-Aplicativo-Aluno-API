using Dapper;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ResponsavelEOLRepositorio : IResponsavelEOLRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public ResponsavelEOLRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private SqlConnection CriaConexao() => new SqlConnection(variaveisGlobaisOptions.EolConnection);
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

        public async Task<IEnumerable<ResponsavelAlunoDetalhadoEolDto>> ObterDadosReponsavelPorCpf(string cpfResponsavel)
        {
            var sql =
                 @"
                select 
                        a.cd_aluno CodigoAluno,
                        ra.tp_pessoa_responsavel TipoPessoa,
                        ra.nm_responsavel Nome,
                        ra.nr_rg_responsavel NumeroRG,
                        ra.cd_digito_rg_responsavel DigitoRG,
                        ra.sg_uf_rg_responsavel UfRG,
                        ra.in_cpf_responsavel_confere CPFConfere,
                        ra.cd_ddd_celular_responsavel DDDCelular,
                        ra.nr_celular_responsavel NumeroCelular,
                        ra.cd_tipo_turno_celular TipoTurnoCelular,
                        coalesce(ra.cd_ddd_telefone_fixo_responsavel,'') DDDTelefoneFixo,
                        coalesce(ra.nr_telefone_fixo_responsavel,'') NumeroTelefoneFixo,
                        ra.cd_tipo_turno_fixo TipoTurnoTelefoneFixo,
                        coalesce(ra.cd_ddd_telefone_comercial_responsavel,'') DDDTelefoneComercial,
                        coalesce(ra.nr_telefone_comercial_responsavel,'') NumeroTelefoneComercial,
                        ra.cd_tipo_turno_comercial TipoTurnoTelefoneComercial,
                        ra.in_autoriza_envio_sms AutorizaEnvioSMS,
                        ra.email_responsavel Email,
                        ra.nm_mae_responsavel NomeMae,
                        ra.dt_nascimento_mae_responsavel DataNascimentoMae,
                        ra.cd_cpf_responsavel as Cpf
					from v_aluno_cotic(nolock) a
					inner join responsavel_aluno(nolock) ra on ra.cd_aluno = a.cd_aluno 
                    INNER JOIN (select cd_aluno , cd_matricula
                              from v_matricula_cotic vmc 
                              inner join escola esc on esc.cd_escola = vmc.cd_escola
			                  where st_matricula = 1
			                    and (cd_serie_ensino is not null -- turma regular 
			                    or esc.tp_escola in (22, 23))
				                and an_letivo = year(getdate())
				                ) as   matricula
                     on matricula.cd_aluno = a.cd_aluno
                   inner join (
                               select cd_matricula,
				                      cd_turma_escola,
				                      cd_situacao_aluno
                                      from matricula_turma_escola (nolock)
                              ) mte 
                  on mte.cd_matricula = matricula.cd_matricula
                 and mte.cd_situacao_aluno in (1, 6, 10, 13)
                WHERE ra.cd_cpf_responsavel = @cpfResponsavel 
                  AND ra.dt_fim IS NULL  
                  AND ra.cd_cpf_responsavel IS NOT NULL";

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var responsavelEol = await conn.QueryAsync<ResponsavelAlunoDetalhadoEolDto>(sql, new { cpfResponsavel });
            await conn.CloseAsync();
            return responsavelEol;
        }
        public async Task<int> AtualizarDadosResponsavel(string codigoAluno, long cpfResponsavel, string email, DateTime dataNascimentoResponsavel, string nomeMae, string dddCelular, string celular)
        {
            var query = @"update responsavel_aluno 
                             set email_responsavel = @email,
                                 dt_nascimento_mae_responsavel = @dataNascimentoResponsavel,
                                 nm_mae_responsavel = @NomeMae,
                                 cd_ddd_celular_responsavel = @dddCelular,
                                 nr_celular_responsavel = @celular,
                                 in_cpf_responsavel_confere = 'S',
                                 in_autoriza_envio_sms = 'S',
                                 cd_tipo_turno_celular = 1,
                                 dt_atualizacao_tabela = @dataAtualizacao
                           where cd_cpf_responsavel = @cpfResponsavel 
                             and cd_aluno = @codigoAluno";

            var parametros = new
            {
                codigoAluno,
                cpfResponsavel,
                email,
                dataNascimentoResponsavel,
                nomeMae,
                dddCelular,
                celular,
                dataAtualizacao = DateTime.Now
            };

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var resultado = await conn.ExecuteAsync(query, parametros);
            await conn.CloseAsync();

            return resultado;
        }

        public async Task<UsuarioDadosDetalhesDto> ObterPorCpfParaDetalhes(string cpf)
        {

            var query = @"select ra.nm_responsavel as Nome, ra.cd_cpf_responsavel as Cpf, ra.dt_nascimento_mae_responsavel as DataNascimento, ra.nm_mae_responsavel as NomeMae, ra.email_responsavel as Email, 
				                ra.cd_ddd_celular_responsavel + ra.nr_celular_responsavel as Celular, ra.dt_atualizacao_tabela as UltimaAtualizacao
                                from responsavel_aluno  ra
				 where ra.cd_cpf_responsavel = @cpf";

            using var conn = CriaConexao();
            await conn.OpenAsync();
            var responsavelEol = await conn.QueryFirstOrDefaultAsync<UsuarioDadosDetalhesDto>(query, new { cpf });
            await conn.CloseAsync();
          
            return responsavelEol;

        }
    }
}
