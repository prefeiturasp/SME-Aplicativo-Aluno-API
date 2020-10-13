using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IAtualizarNotificacaoUseCase
    {
        Task<AtualizacaoNotificacaoResposta> Executar(NotificacaoSgpDto notificacao);
    }
}
