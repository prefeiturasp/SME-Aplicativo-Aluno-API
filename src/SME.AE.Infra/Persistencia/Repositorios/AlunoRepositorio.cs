using Dapper;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum;
using SME.AE.Infra.Persistencia.Consultas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class AlunoRepositorio : IAlunoRepositorio
    {
        public AlunoRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
        }
        private readonly string whereReponsavelAluno = @" WHERE responsavel.cd_cpf_responsavel = @cpf 
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";

        private readonly string whereAlunoCodigo = @" WHERE aluno.cd_aluno = @codigoAluno
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";

        private readonly string whereDreUeReponsavelAluno = @" WHERE responsavel.cd_cpf_responsavel = @cpf 
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND dre.cd_unidade_educacao = @codigoDre
                                                           AND vue.cd_unidade_educacao = @codigoUe
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;

        public async Task<List<AlunoRespostaEol>> ObterDadosAlunos(string cpf)
        {
            try
            {
                using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                var listaAlunos = await conexao.QueryAsync<AlunoRespostaEol>($"{AlunoConsultas.ObterDadosAlunos} {whereReponsavelAluno}", new { cpf });
                return listaAlunos.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<List<AlunoRespostaEol>> ObterDadosAlunosPorDreUeCpfResponsavel(string codigoDre, long codigoUe, string cpf)
        {
            try
            {
                using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                var listaAlunos = await conexao.QueryAsync<AlunoRespostaEol>($"{AlunoConsultas.ObterDadosAlunos} {whereDreUeReponsavelAluno}", new { codigoDre, codigoUe, cpf });
                return listaAlunos.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<List<CpfResponsavelAlunoEol>> ObterCpfsDeResponsaveis(string codigoDre, string codigoUe)
        {
            try
            {
                var query = new StringBuilder();
                query.AppendLine($"{AlunoConsultas.ObterCpfsResponsaveis}");

                if (!string.IsNullOrWhiteSpace(codigoDre))
                    query.AppendLine(" AND dre.cd_unidade_educacao = @codigoDre ");

                if (!string.IsNullOrWhiteSpace(codigoUe))
                    query.AppendLine(" AND vue.cd_unidade_educacao = @codigoUe");

                using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                var cpfsResponsaveis = await conexao.QueryAsync<CpfResponsavelAlunoEol>($"{query}", new { codigoDre, codigoUe });
                return cpfsResponsaveis.ToList();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<IEnumerable<AlunoTurmaEol>> ObterAlunosTurma(long codigoTurma)
        {
            var sql =
                @"
				select 
					mte.nr_chamada_aluno NumeroChamada,
					a.nm_aluno NomeAluno,
					a.cd_aluno CodigoEOLAluno,
					coalesce(ra.cd_cpf_responsavel, 0) Cpf,
					ra.nm_responsavel NomeResponsavel,
					ra.tp_pessoa_responsavel TipoResponsavel,
					ra.cd_ddd_celular_responsavel DDDCelular, 
					ra.nr_celular_responsavel Celular,
					ra.cd_ddd_telefone_fixo_responsavel DDDFixo,
					ra.nr_telefone_fixo_responsavel TelefoneFixo,
					mte.cd_situacao_aluno SituacaoAluno,
					mte.dt_situacao_aluno DataSituacaoAluno
				from v_aluno_cotic(nolock) a
				inner join responsavel_aluno(nolock) ra on ra.cd_aluno = a.cd_aluno 
				inner join v_matricula_cotic(nolock) m on m.cd_aluno = a.cd_aluno 
				inner join matricula_turma_escola(nolock) mte on mte.cd_matricula = m.cd_matricula 
				inner join turma_escola(nolock) te on te.cd_turma_escola = mte.cd_turma_escola
				where 
		            te.cd_tipo_turma = 1 and
		            ra.dt_fim IS NULL and
		            te.cd_turma_escola = @codigoTurma
                union
				select 
					mte.nr_chamada_aluno NumeroChamada,
					a.nm_aluno NomeAluno,
					a.cd_aluno CodigoEOLAluno,
					coalesce(ra.cd_cpf_responsavel, 0) Cpf,
					ra.nm_responsavel NomeResponsavel,
					ra.tp_pessoa_responsavel TipoResponsavel,
					ra.cd_ddd_celular_responsavel DDDCelular, 
					ra.nr_celular_responsavel Celular,
					ra.cd_ddd_telefone_fixo_responsavel DDDFixo,
					ra.nr_telefone_fixo_responsavel TelefoneFixo,
					mte.cd_situacao_aluno SituacaoAluno,
					mte.dt_situacao_aluno DataSituacaoAluno
				from v_aluno_cotic(nolock) a
				inner join responsavel_aluno(nolock) ra on ra.cd_aluno = a.cd_aluno 
				inner join v_historico_matricula_cotic(nolock) m on m.cd_aluno = a.cd_aluno 
				inner join historico_matricula_turma_escola(nolock) mte on mte.cd_matricula = m.cd_matricula 
				inner join turma_escola(nolock) te on te.cd_turma_escola = mte.cd_turma_escola
				where 
		            te.cd_tipo_turma = 1 and
		            ra.dt_fim IS NULL and
		            te.cd_turma_escola = @codigoTurma
                ";
            try
            {
                using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                var listaAlunos = await conexao.QueryAsync<AlunoTurmaEol>(sql, new { codigoTurma });

                var alunosRetorno = listaAlunos
                    .GroupBy(
                        key => key.CodigoEOLAluno,
                        value => value,
                        (key, value) =>
                        {
                            var aluno =
                                value
                                .OrderByDescending(aluno => aluno.DataSituacaoAluno)
                                .First();
                            return aluno;
                        }
                    )
                    .ToArray();
                return alunosRetorno;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }

        public async Task<AlunoRespostaEol> ObterDadosAlunoPorCodigo(long codigoAluno)
        {
            try
            {
                using var conexao = new SqlConnection(variaveisGlobaisOptions.EolConnection);
                return await conexao.QueryFirstOrDefaultAsync<AlunoRespostaEol>($"{AlunoConsultas.ObterDadosAlunos} {whereAlunoCodigo}", new { codigoAluno });
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}