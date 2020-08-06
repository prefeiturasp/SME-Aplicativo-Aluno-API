using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IBaseRepositorio<T> where T : EntidadeBase
    {
        IEnumerable<T> Listar();

        T ObterPorId(long id);

        Task<T> ObterPorIdAsync(long id);

        long Salvar(T entidade);

        Task<long> SalvarAsync(T entidade);

        void Remover(long id);

        void Remover(T entidade);

        Task<IEnumerable<T>> ListarAsync();

        Task RemoverAsync(long id);

        Task RemoverAsync(T entidade);
    }
}
