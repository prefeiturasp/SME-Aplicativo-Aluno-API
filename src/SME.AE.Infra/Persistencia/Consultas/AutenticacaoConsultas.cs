using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AutenticacaoConsultas
    {
        internal static string ObterResponsavel = @"
        SELECT 
            responsavel.cd_identificador_responsavel AS Id,
            RTRIM(LTRIM(responsavel.nm_responsavel)) AS Nome,
            RTRIM(LTRIM(responsavel.cd_cpf_responsavel)) AS Cpf,
            RTRIM(LTRIM(responsavel.email_responsavel)) AS Email
            FROM v_aluno_cotic aluno
            INNER JOIN responsavel_aluno responsavel
                ON aluno.cd_aluno = responsavel.cd_aluno ";
    }
}
