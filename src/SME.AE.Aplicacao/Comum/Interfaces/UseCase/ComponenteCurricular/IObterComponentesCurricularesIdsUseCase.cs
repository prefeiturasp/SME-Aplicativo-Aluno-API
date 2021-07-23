using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IObterComponentesCurricularesIdsUseCase
    {
        Task<IEnumerable<ComponenteCurricularDto>> Executar(AlunoBimestresTurmaDto notaAlunoDto);
    }
}