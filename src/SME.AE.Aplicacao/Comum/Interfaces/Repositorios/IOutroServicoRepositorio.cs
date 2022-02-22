using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IOutroServicoRepositorio : IBaseRepositorio<OutroServico>
    {
        Task<IEnumerable<OutroServicoDto>> LinksPrioritarios();
        Task<IEnumerable<OutroServicoDto>> Links();
    }
}
