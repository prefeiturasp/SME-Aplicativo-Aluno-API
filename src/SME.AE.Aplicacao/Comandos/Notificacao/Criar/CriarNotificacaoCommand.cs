using MediatR;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Criar
{
    public class CriarNotificacaoCommand : IRequest<Unit>
    {
        public Dominio.Entidades.Notificacao Notificacao { get; set; }

        public CriarNotificacaoCommand(Dominio.Entidades.Notificacao notificacao)
        {
            Notificacao = notificacao;
        }
    }
}
