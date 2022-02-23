using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IRemoveNotificacaoPorIdUseCase
    {
        Task<bool> Executar(int id);
    }
}
