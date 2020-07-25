using MediatR;
using Microsoft.Practices.ObjectBuilder2;
using SME.AE.Aplicacao.Comandos.GrupoNotificacao.ObterPorResponsavel;
using SME.AE.Aplicacao.Comandos.Notificacao.ObterPorGrupo;
using SME.AE.Aplicacao.Comum.Enumeradores;
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

            IEnumerable<NotificacaoResposta> lista = await mediator.Send(new ObterNotificacaoPorGrupoCommand(grupos.JoinStrings(","), usuario));

            return AdicionaCategoriasERetornaLista(lista);
        }

        private static IEnumerable<NotificacaoResposta> AdicionaCategoriasERetornaLista(IEnumerable<NotificacaoResposta> lista)
        {
            var listaRetorno = new List<NotificacaoResposta>();

            foreach (var item in lista)
            {

                if (item.TipoComunicado == TipoComunicado.SME)
                    item.CategoriaNotificacao = "SME";
                else if (item.TipoComunicado == TipoComunicado.DRE ||
                         item.TipoComunicado == TipoComunicado.UE ||
                         item.TipoComunicado == TipoComunicado.UEMOD)
                    item.CategoriaNotificacao = "UE";
                else if (item.TipoComunicado == TipoComunicado.TURMA ||
                         item.TipoComunicado == TipoComunicado.ALUNO)
                    item.CategoriaNotificacao = "TURMA";

                listaRetorno.Add(item);

            }
            return listaRetorno;
        }
    }
}
