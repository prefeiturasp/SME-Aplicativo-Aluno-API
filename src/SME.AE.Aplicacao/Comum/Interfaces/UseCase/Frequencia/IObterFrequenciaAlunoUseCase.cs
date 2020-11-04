using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterFrequenciaAlunoUseCase
    {
        Task<IEnumerable<FrequenciaAlunoResposta>> Executar(int anoLetivo, string codigoUe, long codigoTurma, string codigoAluno);
    }
}

