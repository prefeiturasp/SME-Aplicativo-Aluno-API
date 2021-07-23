using SME.AE.Comum;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public interface IAtualizarDadosUsuarioProdamUseCase
    {
        Task<bool> Executar(MensagemRabbit mensagemRabbit);
    }
}
