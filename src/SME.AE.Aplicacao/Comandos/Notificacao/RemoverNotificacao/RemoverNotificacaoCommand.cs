using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoCommand : IRequest<string[]>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoCommand(long[] ids)
        {
            Ids = ids;
        }
    }
}