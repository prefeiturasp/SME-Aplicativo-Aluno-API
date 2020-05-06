using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterDoUsuarioLogadoUseCase
    {
        public static async Task<IEnumerable<NotificacaoPorUsuario>> Executar(IMediator mediator, string usuario)
        {
            List<string> grupos = await mediator.Send(new ObterGrupoNotificacaoPorResponsavelCommand(usuario));
            return await mediator.Send(new ObterNotificacaoPorGrupoCommand(grupos.JoinStrings(","), usuario));
        }
    }
}
