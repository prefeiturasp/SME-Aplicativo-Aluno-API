using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comandos.Token.Criar;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class CriarNotificacaoUseCase
    {
        public static async Task<Dominio.Entidades.Notificacao> Executar(IMediator mediator, Dominio.Entidades.Notificacao notificacao)
        {
            return await mediator.Send(new CriarNotificacaoCommand(notificacao));
        }
    }
}
