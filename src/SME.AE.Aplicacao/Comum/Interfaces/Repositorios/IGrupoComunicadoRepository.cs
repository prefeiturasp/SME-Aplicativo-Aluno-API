using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IGrupoComunicadoRepository
    {
        public Task<IEnumerable<GrupoComunicado>> ObterPorIds(string ids);
        
        public Task<IEnumerable<GrupoComunicado>> ObterTodos();
    }
}