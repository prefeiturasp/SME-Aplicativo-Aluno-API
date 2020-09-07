using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterUsuarioUseCase
    {
        Task<UsuarioDto> Executar(string codigoDre, long codigoUe, string cpf);
    }
}