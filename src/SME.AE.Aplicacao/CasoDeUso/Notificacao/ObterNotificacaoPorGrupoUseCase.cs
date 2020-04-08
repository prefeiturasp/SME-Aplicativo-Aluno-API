using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterNotificacaoPorGrupoUseCase
    {
        public static async Task<IEnumerable<Dominio.Entidades.Notificacao>> Executar(IMediator mediator, string grupo)
        {
            return await mediator.Send(new ObterNotificacaoPorGrupoCommand(grupo));
        }
    }
}