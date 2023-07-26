using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoTurmaPorNotificacoesIdsCommand : IRequest<bool>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoTurmaPorNotificacoesIdsCommand(long[] ids)
        {
            Ids = ids;
        }
    }
}
