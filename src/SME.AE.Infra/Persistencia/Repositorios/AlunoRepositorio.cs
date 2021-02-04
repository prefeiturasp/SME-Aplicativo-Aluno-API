using Dapper;
using Newtonsoft.Json;
using Sentry;
using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
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
        private readonly ICacheRepositorio cacheRepositorio;

        public AlunoRepositorio(ICacheRepositorio cacheRepositorio)
        {
            this.cacheRepositorio = cacheRepositorio;
        }

        private readonly string whereReponsavelAluno = @" WHERE responsavel.cd_cpf_responsavel = @cpf 
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";

        private readonly string whereDreUeReponsavelAluno = @" WHERE responsavel.cd_cpf_responsavel = @cpf 
                                                           AND responsavel.dt_fim IS NULL  
                                                           AND dre.cd_unidade_educacao = @codigoDre
                                                           AND vue.cd_unidade_educacao = @codigoUe
                                                           AND responsavel.cd_cpf_responsavel IS NOT NULL
                                                           AND aluno.cd_tipo_sigilo is null";
        public async Task<List<AlunoRespostaEol>> ObterDadosAlunos(string cpf)
        {
            try
            {
                var chaveCache = $"dadosAlunos-{cpf}";
                var dadosAlunos = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dadosAlunos))
                    return JsonConvert.DeserializeObject<List<AlunoRespostaEol>>(dadosAlunos);

                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
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
                var chaveCache = $"dadosAlunosPorDreUeCpfResponsavel-{codigoDre}-{codigoUe}{cpf}";
                var dadosAlunos = await cacheRepositorio.ObterAsync(chaveCache);
                if (!string.IsNullOrWhiteSpace(dadosAlunos))
                    return JsonConvert.DeserializeObject<List<AlunoRespostaEol>>(dadosAlunos);

                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
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

                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
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
                using var conexao = new SqlConnection(ConnectionStrings.ConexaoEol);
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
    }
}