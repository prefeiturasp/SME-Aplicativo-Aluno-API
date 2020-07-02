using MediatR;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class ObterNotificacaoDoUsuarioLogadoUseCase : IObterNotificacaoDoUsuarioLogadoUseCase
    {
        private readonly IMediator mediator;

        public ObterNotificacaoDoUsuarioLogadoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<NotificacaoResposta>> Executar(string usuario)
        {
            List<string> grupos = await mediator.Send(new ObterGrupoNotificacaoPorResponsavelCommand(usuario));

            return await mediator.Send(new ObterNotificacaoPorGrupoCommand(grupos.JoinStrings(","), usuario));
        }
    }
}
