using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface ISolicitarBoletimAlunoUseCase
    {
        Task<bool> Executar(SolicitarBoletimAlunoDto solicitarBoletim);
    }
}
