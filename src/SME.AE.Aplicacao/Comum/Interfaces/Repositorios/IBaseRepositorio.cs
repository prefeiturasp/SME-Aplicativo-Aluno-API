using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IBaseRepositorio<T> where T : EntidadeBase
    {

        Task<T> ObterPorIdAsync(long id);

        Task<long> SalvarAsync(T entidade);

        Task<IEnumerable<T>> ListarAsync();

        Task RemoverAsync(long id);

        Task RemoverAsync(T entidade);
    }
}
