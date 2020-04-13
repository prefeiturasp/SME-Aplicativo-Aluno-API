using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Consultas
{
  public static  class AlunoConsultas
    {
		internal static string ObterDadosAlunos = @"
          SELECT 
		    
             aluno.cd_aluno CodigoEol,
			 aluno.nm_aluno Nome,
			 aluno.nm_social_aluno NomeSocial,
			 rtrim(ltrim(tesc.sg_tp_escola)) DescricaoTipoEscola,
		     ltrim(rtrim(vue.nm_unidade_educacao))     Escola,
			 dre.nm_exibicao_unidade SiglaDre,
			 te.dc_turma_escola Turma,
			 tesc.tp_escola CodigoTipoEscola

			FROM v_aluno_cotic aluno
			INNER JOIN responsavel_aluno responsavel
			ON aluno.cd_aluno = responsavel.cd_aluno
			INNER JOIN(select cd_aluno,
							   cd_matricula,
							   cd_escola
						  from v_matricula_cotic
						  where st_matricula = 1

							and cd_serie_ensino is not null -- turma regular
							and an_letivo = year(getdate())
							) as   matricula
			on matricula.cd_aluno = aluno.cd_aluno
			   inner join(
						   select cd_matricula,
								  cd_turma_escola,
								  cd_situacao_aluno
								  from matricula_turma_escola (nolock)

		  ) mte on mte.cd_matricula = matricula.cd_matricula
			and mte.cd_situacao_aluno in (1, 6, 10, 13)
            inner join v_cadastro_unidade_educacao(nolock) vue on vue.cd_unidade_educacao = matricula.cd_escola
		   inner join escola esc on esc.cd_escola = vue.cd_unidade_educacao
		   inner join tipo_escola(nolock) tesc on tesc.tp_escola = esc.tp_escola
		  and tesc.tp_escola in (1,2,3,4,10,11,12,13,14,15,16,17,18,19,20,22,23,24,25,26,27,28,29,30,31)
            inner join v_cadastro_unidade_educacao(nolock) dre
			  on dre.cd_unidade_educacao = vue.cd_unidade_administrativa_referencia
		   inner join turma_escola(nolock) te on te.cd_turma_escola = mte.cd_turma_escola ";
	}
}
