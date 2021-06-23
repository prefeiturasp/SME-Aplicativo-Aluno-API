using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IObterFrequenciaGlobalAlunoUseCase
    {
        Task<bool> Executar(FiltroFrequenciaGlobalAlunoDto filtro);
    }
}