using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Interfaces
{
    public interface IUseCase<in TParameter, TResponse>
    {
        Task<TResponse> Executar(TParameter param);
    }
}
