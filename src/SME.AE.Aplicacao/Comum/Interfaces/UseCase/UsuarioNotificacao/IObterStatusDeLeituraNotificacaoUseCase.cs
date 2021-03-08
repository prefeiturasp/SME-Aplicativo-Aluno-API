using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterStatusDeLeituraNotificacaoUseCase
    {
        Task<IEnumerable<StatusNotificacaoUsuario>> Executar(List<long> notificacaoId, long codigoAluno);
    }
}
