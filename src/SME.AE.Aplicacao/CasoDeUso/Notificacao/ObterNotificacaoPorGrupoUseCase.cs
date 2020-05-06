using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterNotificacaoPorGrupoUseCase
    {
        public static async Task<IEnumerable<NotificacaoPorUsuario>> Executar(IMediator mediator, string grupo, string cpf)
        {
            return await mediator.Send(new ObterNotificacaoPorGrupoCommand(grupo, cpf));
        }
    }
}