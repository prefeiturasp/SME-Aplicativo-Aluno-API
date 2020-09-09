﻿using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IAlunoRepositorio
    {
        Task<List<AlunoRespostaEol>> ObterDadosAlunos(string cpf);

        Task<List<AlunoRespostaEol>> ObterDadosAlunosPorDreUeCpfResponsavel(string codigoDre, long codigoUe, string cpf);
    }
}
