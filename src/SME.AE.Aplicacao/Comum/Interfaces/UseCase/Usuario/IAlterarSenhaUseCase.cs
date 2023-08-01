using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario
{
    public interface IAlterarSenhaUseCase
    {
        Task<RespostaApi> Executar(AlterarSenhaDto alterarSenhaDto, string senhaAntiga);
    }
}
