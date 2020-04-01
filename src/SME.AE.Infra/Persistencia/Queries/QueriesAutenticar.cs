using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Queries
{
    public static class QueriesAutenticar
    {
        internal static string ObterAlunosDoResponsavel = @"
            SELECT Aluno.DataNascimento
            FROM v_aluno";
    }
}
