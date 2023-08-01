using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface ISolicitarRelatorioRaaUseCase
    {
        Task<bool> Executar(SolicitarRelatorioRaaDto filtro);
    }
}
