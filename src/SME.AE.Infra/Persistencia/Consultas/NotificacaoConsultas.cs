﻿namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class NotificacaoConsultas
    {
        public static string Select = @"
            SELECT Id, Mensagem, Titulo, Grupo, DataEnvio, DataExpiracao,
                   CriadoEm, CriadoPor, AlteradoEm, AlteradoPor
            FROM Notificacao 
        ";

        public static string GruposDoResponsavel = @"
            from dbo.responsavel_aluno ra
                inner join dbo.aluno a on a.cd_aluno = ra.cd_aluno
                inner join dbo.v_matricula_cotic mc on mc.cd_aluno = a.cd_aluno 
                inner join dbo.serie_ensino se on mc.cd_serie_ensino = se.cd_serie_ensino
            where ra.cd_cpf_responsavel = @cpf
            group by se.cd_ciclo_ensino, se.cd_etapa_ensino
            order by se.cd_ciclo_ensino, se.cd_etapa_ensino desc;
        ";
    }
}
