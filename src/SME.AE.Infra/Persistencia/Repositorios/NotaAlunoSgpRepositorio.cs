using Dapper;
using Npgsql;
using Sentry;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Servicos;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotaAlunoSgpRepositorio : INotaAlunoSgpRepositorio
    {
        private readonly VariaveisGlobaisOptions variaveisGlobaisOptions;
		private readonly IServicoTelemetria servicoTelemetria;

        public NotaAlunoSgpRepositorio(VariaveisGlobaisOptions variaveisGlobaisOptions,
			IServicoTelemetria servicoTelemetria)
        {
            this.variaveisGlobaisOptions = variaveisGlobaisOptions ?? throw new ArgumentNullException(nameof(variaveisGlobaisOptions));
			this.servicoTelemetria = servicoTelemetria;
		}

        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(variaveisGlobaisOptions.SgpConnection);

        public async Task<IEnumerable<NotaAlunoSgpDto>> ObterNotaAlunoSgp(int desdeAnoLetivo, long ueId, int pagina, int quantidadeRegistrosPagina)
        {
			var sql = @"select distinct 
								coalesce(con.AnoLetivo, fec.AnoLetivo) AnoLetivo,
								coalesce(con.CodigoUe, fec.CodigoUe) CodigoUe,
								coalesce(con.CodigoTurma, fec.CodigoTurma) CodigoTurma,
								coalesce(con.Bimestre, fec.Bimestre) Bimestre,
								coalesce(con.CodigoAluno, fec.CodigoAluno) CodigoAluno,
								coalesce(con.CodigoComponenteCurricular, fec.CodigoComponenteCurricular) CodigoComponenteCurricular,
								coalesce(con.ComponenteCurricular, fec.ComponenteCurricular) ComponenteCurricular,
								coalesce(con.Nota, fec.Nota) Nota,
								coalesce(con.NotaDescricao, fec.NotaDescricao) NotaDescricao,
								con.RecomendacoesAluno,
								con.RecomendacoesFamilia,
								ccra.recomendacao RecomendacoesAlunoComplementares,
								ccrf.recomendacao RecomendacoesFamiliaComplementares,
								coalesce(con.cca_id, fec.cca_id, 0) cca_id
							from 
								(
									select 
									distinct 
										t.ano_letivo 		AnoLetivo,
										ue.ue_id   			CodigoUe,
										t.turma_id 			CodigoTurma,
										coalesce(
											pe.bimestre,
											0)				Bimestre,
										fa.aluno_codigo 	CodigoAluno,
										cc.id 				CodigoComponenteCurricular,
										coalesce(cc.descricao_sgp, cc.descricao) 	ComponenteCurricular,										
										coalesce (
											fn.nota::varchar, cv.valor,
											sv.valor::varchar, sv.descricao,
											''
										) 					Nota,
										trim(coalesce (
											fn.nota::varchar, cv.descricao,
											sv.descricao, '	',
											''
										), '	') 					NotaDescricao,																
										cca.recomendacoes_aluno RecomendacoesAluno,
										cca.recomendacoes_familia RecomendacoesFamilia,
										cca.id cca_id
									from 
										fechamento_turma ft
									inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id 
									inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id 
									inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id
									inner join turma t on t.id = ft.turma_id
									inner join ue on ue.id = t.ue_id 
									inner join conselho_classe cc2 on cc2.fechamento_turma_id = ft.id and not cc2.excluido 
									inner join conselho_classe_aluno cca on cca.aluno_codigo = fa.aluno_codigo and cca.conselho_classe_id = cc2.id and not cca.excluido 
									left join periodo_escolar pe on pe.id = ft.periodo_escolar_id
									left join componente_curricular cc on cc.id = fn.disciplina_id 
									left join conceito_valores cv on cv.id = fn.conceito_id 
									left join sintese_valores sv on sv.id = fn.sintese_id 
									where t.ano_letivo = @desdeAnoLetivo
										and not ft.excluido 
										and not ftd.excluido 
										and not fa.excluido 
										and not fn.excluido
										and ue.id = @ueId
									order by 12
									limit @quantidadeRegistrosPagina
									offset (@pagina - 1) * @quantidadeRegistrosPagina
								) fec
							full outer join
								(
									select 
									distinct 
										t.ano_letivo 		AnoLetivo,
										ue.ue_id   			CodigoUe,
										t.turma_id 			CodigoTurma,
										coalesce(
											pe.bimestre,
											0)				Bimestre,
										cca.aluno_codigo 	CodigoAluno,
										cu.id 				CodigoComponenteCurricular,
										coalesce(
											cu.descricao_sgp, 
											cu.descricao) 	ComponenteCurricular,
										coalesce (
											ccn.nota::varchar, cv.valor,
											''
										) 					Nota,
										trim(coalesce(
											ccn.nota::varchar, cv.descricao,
											''
										), '	') 					NotaDescricao,									
										cca.recomendacoes_aluno RecomendacoesAluno,
										cca.recomendacoes_familia RecomendacoesFamilia,
										cca.id cca_id
									from
										conselho_classe cc 
									inner join conselho_classe_aluno cca on cca.conselho_classe_id = cc.id and not cca.excluido 
									inner join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id and not ccn.excluido
									inner join fechamento_turma ft on ft.id = cc.fechamento_turma_id and not ft.excluido 
									inner join turma t on t.id = ft.turma_id
									inner join ue on ue.id = t.ue_id 
									inner join componente_curricular cu on cu.id = ccn.componente_curricular_codigo 
									left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
									left join conceito_valores cv on cv.id = ccn.conceito_id 
									where t.ano_letivo = @desdeAnoLetivo
									and not cc.excluido
									and ue.id = @ueId
									order by 12
									limit @quantidadeRegistrosPagina
									offset (@pagina - 1) * @quantidadeRegistrosPagina
								) con 
							on 	con.anoletivo = fec.anoletivo
							and con.codigoue  = fec.codigoue
							and con.codigoturma = fec.codigoturma
							and con.bimestre = fec.bimestre
							and con.codigocomponentecurricular = fec.codigocomponentecurricular
							and con.codigoaluno = fec.codigoaluno
							left join conselho_classe_aluno_recomendacao ccar on coalesce(con.cca_id, fec.cca_id, 0) = ccar.conselho_classe_aluno_id
							left join conselho_classe_recomendacao ccra on ccar.conselho_classe_recomendacao_id = ccra.id and ccra.tipo = 2
							left join conselho_classe_recomendacao ccrf on ccar.conselho_classe_recomendacao_id = ccra.id and ccrf.tipo = 1
							where ((con.RecomendacoesAluno = '' or con.RecomendacoesAluno is null) and 
								   (con.RecomendacoesFamilia = '' or con.RecomendacoesFamilia is null)) and
								  ccra.id is not null and
								  ccrf.id is not null;";

            var parametros = new { desdeAnoLetivo, ueId, pagina, quantidadeRegistrosPagina };

			try
            {
                using var conexao = CriaConexao();

                conexao.Open();

				var notaAlunosSgp = await servicoTelemetria.RegistrarComRetornoAsync<NotaAlunoSgpDto>(async () => await SqlMapper.QueryAsync<NotaAlunoSgpDto>(conexao, sql, parametros),
					"quyer", "Query SGP", sql, parametros.ToString());

                conexao.Close();

                return notaAlunosSgp;
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }
        }
    }
}