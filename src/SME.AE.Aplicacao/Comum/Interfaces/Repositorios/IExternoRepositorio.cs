using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IExternoRepositorio<T> where T : class
    {
        IEnumerable<T> Listar();

        T ObterPorId(long id);

        Task<T> ObterPorIdAsync(long id);
        
        Task<IEnumerable<T>> ListarAsync();

        Task<IEnumerable<T>> QueryAsync(string query, object parametros);

        Task<T> QueryFirstOrDefaultAsync(string query, object parametros);

        IEnumerable<T> Query(string query, object parametros);

        T QueryFirstOrDefault(string query, object parametros);
    }
}
