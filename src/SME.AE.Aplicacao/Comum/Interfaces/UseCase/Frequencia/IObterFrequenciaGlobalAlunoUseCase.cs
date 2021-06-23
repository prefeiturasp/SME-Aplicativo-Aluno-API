using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IObterFrequenciaGlobalAlunoUseCase
    {
        Task<double?> Executar(FiltroFrequenciaGlobalAlunoDto filtro);
    }
}