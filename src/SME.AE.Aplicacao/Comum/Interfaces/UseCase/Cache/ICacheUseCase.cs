using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface ICacheUseCase
    {
        Task<string> Executar(string cpf);
    }
}
