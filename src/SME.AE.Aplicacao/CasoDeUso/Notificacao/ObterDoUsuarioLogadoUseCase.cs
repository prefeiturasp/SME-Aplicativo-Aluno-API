using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterDoUsuarioLogadoUseCase
    {
        public static async Task<IEnumerable<Dominio.Entidades.Notificacao>> Executar(IMediator mediator, string usuario)
        {
            IEnumerable grupos = await mediator.Send(new ObterGrupoNotificacaoPorResponsavelCommand(usuario));
            
            return new List<Dominio.Entidades.Notificacao>();
        }
    }
}