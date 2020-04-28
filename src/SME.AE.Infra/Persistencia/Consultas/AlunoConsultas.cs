using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Consultas
{
  public static  class AlunoConsultas
    {
		internal static string ObterDadosAlunos = @"
		SELECT aluno.cd_aluno                        CodigoEol, 
               aluno.nm_aluno                        Nome, 
               aluno.nm_social_aluno                 NomeSocial, 
               aluno.dt_nascimento_aluno             DataNascimento, 
               Rtrim(Ltrim(tesc.sg_tp_escola))       DescricaoTipoEscola, 
               Ltrim(Rtrim(vue.nm_unidade_educacao)) Escola, 
               dre.nm_exibicao_unidade               SiglaDre, 
               te.dc_turma_escola                    Turma, 
               tesc.tp_escola                        CodigoTipoEscola, 
               mte.situacaomatricula                 SituacaoMatricula, 
               mte.dt_situacao_aluno                 DataSituacaoMatricula, 
               etapa_ensino.cd_etapa_ensino AS CodigoEtapaEnsino,
               ciclo_ensino.cd_ciclo_ensino AS CodigoCicloEnsino
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
               INNER JOIN escola esc 
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
               INNER JOIN serie_ensino 
                       ON matricula.cd_serie_ensino = serie_ensino.cd_serie_ensino 
               INNER JOIN etapa_ensino 
                       ON serie_ensino.cd_etapa_ensino = etapa_ensino.cd_etapa_ensino 
               INNER JOIN ciclo_ensino 
                       ON serie_ensino.cd_ciclo_ensino = ciclo_ensino.cd_ciclo_ensino  ";
	}
}
