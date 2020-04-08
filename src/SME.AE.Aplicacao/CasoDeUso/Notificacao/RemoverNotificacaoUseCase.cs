using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class RemoverNotificacaoUseCase
    {
        public static async Task<bool> Executar(IMediator mediator, Dominio.Entidades.Notificacao notificacao)
        {
            return await mediator.Send(new RemoverNotificacaoCommand(notificacao));
        }
    }
}