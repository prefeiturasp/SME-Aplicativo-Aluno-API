using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface ICriarNotificacaoUseCase
    {
        Task EnviarNotificacaoImediataAsync(NotificacaoSgpDto notificacao);
        Task<NotificacaoSgpDto> Executar(NotificacaoSgpDto notificacao);
    }
}
