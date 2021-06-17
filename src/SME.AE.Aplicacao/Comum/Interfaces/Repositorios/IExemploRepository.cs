using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IExemploRepository
    {
        Task<IEnumerable<string>> ObterNomesDeExemplos();
    }
}