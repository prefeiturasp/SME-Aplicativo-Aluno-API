using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IObterFrequenciaGlobalAlunoUseCase
    {
        Task<FrequenciaGlobalDto> Executar(FiltroFrequenciaGlobalAlunoDto filtro);
    }
}