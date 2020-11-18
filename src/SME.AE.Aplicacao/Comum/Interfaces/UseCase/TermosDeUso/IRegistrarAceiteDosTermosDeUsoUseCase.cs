using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IRegistrarAceiteDosTermosDeUsoUseCase
    {
        Task<bool> Executar(RegistrarAceiteDosTermosDeUsoDto aceite);
    }
}
