using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IObterBimestresLiberacaoBoletimAlunoUseCase
    {
        Task<int[]> Executar(string turmaCodigo);
    }
}
