﻿using SME.AE.Aplicacao.Comum.Modelos.Resposta.Dre;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IDreSgpRepositorio
    {
        Task<DreResposta> ObterNomeAbreviadoDrePorCodigo(string codigoDre);
    }
}