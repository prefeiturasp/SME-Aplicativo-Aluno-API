using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Dominio.Entidades;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface ICriarNotificacaoUseCase
    {
        Task EnviarNotificacaoImediataAsync(NotificacaoSgpDto notificacao);
        Task<Notificacao> Executar(NotificacaoSgpDto notificacao);
    }
}
