using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class AtualizarNotificacaoUseCase
    {
        public static async Task<Dominio.Entidades.Notificacao> Executar(IMediator mediator, 
            Dominio.Entidades.Notificacao notificacao)
        {
            return await mediator.Send(new AtualizarNotificacaoCommand(notificacao));
        }
    }
}