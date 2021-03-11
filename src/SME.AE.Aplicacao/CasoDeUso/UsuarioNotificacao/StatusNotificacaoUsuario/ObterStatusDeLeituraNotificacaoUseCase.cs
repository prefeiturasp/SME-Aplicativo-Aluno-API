using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterStatusDeLeituraNotificacaoUseCase : IObterStatusDeLeituraNotificacaoUseCase
    {

        private readonly IMediator mediator;

        public ObterStatusDeLeituraNotificacaoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<StatusNotificacaoUsuario>> Executar(List<long> notificacoesId, long codigoAluno)
        {
            return await mediator.Send(new ObterStatusNotificacaoUsuarioQuery(notificacoesId, codigoAluno));
        }
    }
}
