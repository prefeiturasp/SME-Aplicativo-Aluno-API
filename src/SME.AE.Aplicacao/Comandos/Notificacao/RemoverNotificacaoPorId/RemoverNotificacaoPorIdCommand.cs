using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoPorIdCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public RemoverNotificacaoPorIdCommand(int id)
        {
            Id = id;
        }
    }
}
