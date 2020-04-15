using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class RemoveNotificacaoPorIdUseCase
    {
        public static async Task<bool> Executar(IMediator mediator, int id)
        {
            return await mediator.Send(new RemoverNotificacaoPorIdCommand(id));
        }
    }
}
