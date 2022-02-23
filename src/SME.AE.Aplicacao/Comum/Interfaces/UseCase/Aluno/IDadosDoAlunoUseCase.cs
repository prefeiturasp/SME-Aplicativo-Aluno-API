using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IDadosDoAlunoUseCase
    {
        Task<RespostaApi> Executar(string cpf);
    }
}
