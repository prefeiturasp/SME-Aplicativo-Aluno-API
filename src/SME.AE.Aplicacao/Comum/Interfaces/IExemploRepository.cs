using System.Collections.Generic;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IExemploRepository
    {
        IEnumerable<string> ObterNomesDeExemplos();
    }
}