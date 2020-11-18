using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard
{
    public interface IObterTotalUsuariosComAcessoIncompletoUseCase
    {
        Task<long> Executar(string codigoDre, string codigoUe);
    }
}
