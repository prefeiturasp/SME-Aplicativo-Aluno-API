using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard
{
    public interface IObterTotalUsuariosValidosUseCase
    {
        Task<long> Executar(string codigoDre, string codigoUe);
    }
}
