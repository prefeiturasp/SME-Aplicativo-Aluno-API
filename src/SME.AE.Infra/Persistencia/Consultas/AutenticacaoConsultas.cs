using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AutenticacaoConsultas
    {
        internal static string ObterAlunosDoResponsavel = @"
            SELECT Aluno.nm_aluno
            FROM v_aluno_cotic aluno
            INNER JOIN responsavel_aluno responsalvel
                ON aluno.cd_aluno = responsalvel.cd_aluno";
    }
}
