using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase
{
    public interface IObterNotificacaoDoUsuarioLogadoUseCase
    {
        Task<IEnumerable<NotificacaoResposta>> Executar(string cpf, long codigoAluno);
    }
}
