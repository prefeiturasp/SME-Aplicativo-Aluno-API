using SME.AE.Comum;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces
{
    public interface IAtualizarDadosUsuarioEolUseCase 
    {
        Task<bool> Executar(MensagemRabbit mensagemRabbit);
    }
}