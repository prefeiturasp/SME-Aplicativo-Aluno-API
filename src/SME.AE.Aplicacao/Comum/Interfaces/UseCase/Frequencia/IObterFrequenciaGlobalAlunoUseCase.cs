using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IObterFrequenciaGlobalAlunoUseCase
    {
        Task<double?> Executar(FiltroFrequenciaGlobalAlunoDto filtro);
    }
}