using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IValidarUsuarioEhResponsavelDeAlunoUseCase
    {
        Task<bool> Executar(string codigoCpf);
    }
}
