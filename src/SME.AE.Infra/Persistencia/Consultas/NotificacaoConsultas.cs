namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class NotificacaoConsultas
    {
        public static string Select = @"
            SELECT Id, Mensagem, Titulo, Grupo, DataEnvio, DataExpiracao,
                   CriadoEm, CriadoPor, AlteradoEm, AlteradoPor
            FROM Notificacao 
        ";

        // TODO montar cada case when dinamicamente de acordo com o que vem da consulta
        public static string GruposDoResponsavel = @"
            select
	            case when (se.cd_ciclo_ensino in (1, 23) or se.cd_etapa_ensino in (1, 10)) then 1 else 0 end as CEI,
	            case when (se.cd_ciclo_ensino in (1, 23) or se.cd_etapa_ensino in (2, 14)) then 1 else 0 end as EMEI,
	            case when se.cd_ciclo_ensino in (5) then 1 else 0 end as Fundamental,
	            case when se.cd_ciclo_ensino in (6, 8, 9) then 1 else 0 end as Medio,
	            case when se.cd_ciclo_ensino in (3, 11) then 1 else 0 end as EJA
            from dbo.responsavel_aluno ra
            inner join dbo.aluno a on a.cd_aluno = ra.cd_aluno
            inner join dbo.v_matricula_cotic mc on mc.cd_aluno = a.cd_aluno 
            inner join dbo.serie_ensino se on mc.cd_serie_ensino = se.cd_serie_ensino
            where ra.cd_cpf_responsavel = @cpf
            group by se.cd_ciclo_ensino, se.cd_etapa_ensino;
        ";
    }
}
