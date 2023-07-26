using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoAlunoPorNotificacoesIdsCommand : IRequest<bool>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoAlunoPorNotificacoesIdsCommand(long[] ids)
        {
            Ids = ids;
        }
    }
}
