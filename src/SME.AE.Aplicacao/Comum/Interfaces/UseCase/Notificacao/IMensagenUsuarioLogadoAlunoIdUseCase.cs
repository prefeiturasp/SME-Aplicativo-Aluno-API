using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IMensagenUsuarioLogadoAlunoIdUseCase
    {
        Task<NotificacaoResposta> Executar(long id);
    }
}
