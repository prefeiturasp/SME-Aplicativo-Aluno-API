using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class RemoveNotificacaoPorIdUseCase : IRemoveNotificacaoPorIdUseCase
    {
        private readonly IMediator mediator;

        public RemoveNotificacaoPorIdUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(int id)
        {
            return await mediator.Send(new RemoverNotificacaoPorIdCommand(id));
        }
    }
}
