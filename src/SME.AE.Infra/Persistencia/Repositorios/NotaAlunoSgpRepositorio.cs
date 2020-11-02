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
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class NotaAlunoSgpRepositorio : INotaAlunoSgpRepositorio
    {
        private NpgsqlConnection CriaConexao() => new NpgsqlConnection(ConnectionStrings.ConexaoSgp);

        public async Task<IEnumerable<NotaAlunoSgpDto>> ObterNotaAlunoSgp()
        {
            try
            {
                using var conexao = CriaConexao();
                conexao.Open();

                var notaAlunosSgp = await conexao
                    .QueryAsync<NotaAlunoSgpDto>(
						@"
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
							coalesce(
								cc.descricao_sgp, 
								cc.descricao) 	ComponenteCurricular,
							coalesce (
								ccn.nota::varchar, cv2.valor,
								fn.nota::varchar, cv.valor,
								sv.valor,
								''
							) 					Nota,
							cca.id 				conselho_classe_aluno_id
						from 
							fechamento_turma ft
						inner join fechamento_turma_disciplina ftd on ftd.fechamento_turma_id = ft.id 
						inner join fechamento_aluno fa on fa.fechamento_turma_disciplina_id = ftd.id 
						inner join fechamento_nota fn on fn.fechamento_aluno_id = fa.id 
						inner join turma t on t.id = ft.turma_id
						inner join ue on ue.id = t.ue_id 
						left join periodo_escolar pe on pe.id = ft.periodo_escolar_id 
						left join componente_curricular cc on cc.id = fn.disciplina_id
						left join conselho_classe cc2 on cc2.fechamento_turma_id = ft.id 
						left join conselho_classe_aluno cca on cca.aluno_codigo = fa.aluno_codigo and cca.conselho_classe_id = cc2.id and not cca.excluido 
						left join conselho_classe_nota ccn on ccn.conselho_classe_aluno_id = cca.id and ccn.componente_curricular_codigo = cc.id and not ccn.excluido 
						left join conceito_valores cv on cv.id = fn.conceito_id
						left join conceito_valores cv2 on cv2.id = ccn.conceito_id 
						left join sintese_valores sv on sv.id = fn.sintese_id 
						where t.ano_letivo >= 2020
							and (cca.id is null or (cca.id is not null and ccn.id is not null))
							and not ftd.excluido 
							and not fa.excluido 
							and not fn.excluido 
						order by 
							cca.id desc
                        ");
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