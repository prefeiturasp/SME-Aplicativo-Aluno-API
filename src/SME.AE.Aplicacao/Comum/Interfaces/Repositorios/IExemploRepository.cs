using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Config;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IExemploRepository
    {
        Task<IEnumerable<string>> ObterNomesDeExemplos();
    }
}