using FirebaseAdmin.Messaging;
using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo
{
    public class EnviarNotificacaoPorGrupoCommand : IRequest<bool>
    {
        public Message Mensagem { get; set; }

        public EnviarNotificacaoPorGrupoCommand(Message mensagem)
        {
            this.Mensagem = mensagem;
        }
    }
}

