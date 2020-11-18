using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterTermosDeUsoPorCpfUseCase
    {
        Task<RetornoTermosDeUsoDto> Executar(string cpf);
    }
}
