using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagenUsuarioLogadoAlunoIdQuery : IRequest<NotificacaoResposta>
    {
        public long Id { get; set; }
    }
}
