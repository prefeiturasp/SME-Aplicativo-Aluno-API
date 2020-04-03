using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IExemploRepository
    {
        Task<IEnumerable<string>> ObterNomesDeExemplos();
    }
}