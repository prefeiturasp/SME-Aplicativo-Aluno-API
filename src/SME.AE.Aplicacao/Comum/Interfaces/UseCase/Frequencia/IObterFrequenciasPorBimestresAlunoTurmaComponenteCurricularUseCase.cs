using SME.AE.Aplicacao.Comum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IObterFrequenciasPorBimestresAlunoTurmaComponenteCurricularUseCase
    {
        Task<IEnumerable<FrequenciaAlunoDto>> Executar(FrequenciaPorBimestresAlunoTurmaComponenteCurricularDto dto);
    }
}
