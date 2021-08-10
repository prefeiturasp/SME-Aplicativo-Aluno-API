using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoUsuarioCommand : IRequest<bool>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoUsuarioCommand(long[] ids)
        {
            Ids = ids;
        }
    }
}
