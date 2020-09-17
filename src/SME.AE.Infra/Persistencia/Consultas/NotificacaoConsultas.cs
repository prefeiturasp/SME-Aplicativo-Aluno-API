namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class NotificacaoConsultas
    {
        public static string Select = @"
            SELECT Id, Mensagem, Titulo, Grupo, DataEnvio, DataExpiracao,
                   CriadoEm, CriadoPor, AlteradoEm, AlteradoPor, Dre_CodigoEol, Ue_CodigoEol, TipoComunicado, CategoriaNotificacao
            FROM Notificacao 
        ";

        public static string ObterPorUsuarioLogado = @"select 
                                                            distinct 
                                                            N.Id,
	                                                        N.Mensagem,
	                                                        N.Titulo,
	                                                        N.Grupo,
	                                                        N.DataEnvio,
	                                                        N.DataExpiracao,
	                                                        N.CriadoEm,
	                                                        N.CriadoPor,
	                                                        N.AlteradoEm,
	                                                        N.AlteradoPor,
                                                            N.TipoComunicado,
                                                            N.CategoriaNotificacao,
                                                            N.SeriesResumidas,
	                                                        UNL.mensagemvisualizada
                                                        from
	                                                        Notificacao N
                                                        left join usuario_notificacao_leitura UNL on
	                                                        UNL.notificacao_id = N.id ";

        public static string GruposDoResponsavel = @"
            from dbo.responsavel_aluno ra
                inner join dbo.aluno a on a.cd_aluno = ra.cd_aluno
                inner join dbo.v_matricula_cotic mc on mc.cd_aluno = a.cd_aluno 
                inner join dbo.serie_ensino se on mc.cd_serie_ensino = se.cd_serie_ensino
            where ra.cd_cpf_responsavel = @cpf
            group by se.cd_ciclo_ensino, se.cd_etapa_ensino
        ";

        public static string ResponsaveisPorGrupo = @"
                SELECT cd_cpf_responsavel
                FROM responsavel_aluno ra
          INNER JOIN(     SELECT cd_aluno, 
                                 cd_matricula, 
                                 cd_escola, 
                                 cd_serie_ensino 
                          FROM   v_matricula_cotic 
                          WHERE  st_matricula = 1 
                                 AND cd_serie_ensino IS NOT NULL -- turma regular 
                                 AND an_letivo = Year(Getdate())) AS matricula
                                  ON matricula.cd_aluno = ra.cd_aluno
          INNER JOIN(  SELECT   cd_matricula,
	                            cd_situacao_aluno                                                             
                         FROM   matricula_turma_escola (nolock)) mte 
                           ON   mte.cd_matricula = matricula.cd_matricula 
                          AND   mte.cd_situacao_aluno IN ( 1, 6, 10, 13 ) 
         INNER JOIN      dbo.serie_ensino se on matricula.cd_serie_ensino = se.cd_serie_ensino
         INNER JOIN     escola esc on esc.cd_escola = matricula.cd_escola
              WHERE  1 = 1 
                AND cd_cpf_responsavel is not null"
                 ;
    }
}
