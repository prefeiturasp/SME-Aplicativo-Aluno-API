namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AutenticacaoConsultas
    {
        internal static string ObterResponsavel = @"
                SELECT 
                    responsavel.cd_identificador_responsavel AS Id,
                    RTRIM(LTRIM(responsavel.nm_responsavel)) AS Nome,
                    RTRIM(LTRIM(aluno.nm_social_aluno)) AS NomeSocial,
	                responsavel.tp_pessoa_responsavel as TipoResponsavel,
                    responsavel.cd_cpf_responsavel AS Cpf,
                    RTRIM(LTRIM(responsavel.email_responsavel)) AS Email,
                    RTRIM(LTRIM(cd_ddd_celular_responsavel)) AS DDD,
                    RTRIM(LTRIM(nr_celular_responsavel)) AS Celular,
                    RTRIM(LTRIM(responsavel.nm_mae_responsavel)) AS NomeMae,
                    responsavel.dt_nascimento_mae_responsavel AS DataNascimentoResponsavel,
                    aluno.dt_nascimento_aluno AS DataNascimento,
                    aluno.cd_tipo_sigilo as TipoSigilo,
	                aluno.nm_aluno,
                    responsavel.dt_atualizacao_tabela as DataAtualizacao
                FROM v_aluno_cotic aluno
                INNER JOIN responsavel_aluno responsavel
                ON aluno.cd_aluno = responsavel.cd_aluno
                INNER JOIN (select cd_aluno , cd_matricula
                              from v_matricula_cotic vmc 
                              inner join escola esc on esc.cd_escola = vmc.cd_escola
			                  where st_matricula = 1
			                    and (cd_serie_ensino is not null -- turma regular 
			                    or esc.tp_escola in (22, 23))
				                and an_letivo = year(getdate())
				                ) as   matricula
                on matricula.cd_aluno = aluno.cd_aluno
                   inner join (
                               select cd_matricula,
				                      cd_turma_escola,
				                      cd_situacao_aluno
                                      from matricula_turma_escola (nolock)
                              ) mte 
                  on mte.cd_matricula = matricula.cd_matricula
                 and mte.cd_situacao_aluno in (1, 6, 10, 13) ";
    }
}
