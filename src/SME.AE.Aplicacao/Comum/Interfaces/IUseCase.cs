using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IUseCase<T>
    {
        public Task<T> Executar(object request);
    }
}
