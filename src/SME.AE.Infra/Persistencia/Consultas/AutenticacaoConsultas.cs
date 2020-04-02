using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Consultas
{
    public static class AutenticacaoConsultas
    {
        internal static string ObterAlunosDoResponsavel = @"
            SELECT Aluno.DataNascimento
            FROM v_aluno";
    }
}
