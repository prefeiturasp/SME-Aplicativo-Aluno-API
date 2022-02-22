using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IOutroServicoUseCase
    {
        Task<IEnumerable<OutroServicoDto>> LinksPrioritarios();
        Task<IEnumerable<OutroServicoDto>> Links();
    }
}
