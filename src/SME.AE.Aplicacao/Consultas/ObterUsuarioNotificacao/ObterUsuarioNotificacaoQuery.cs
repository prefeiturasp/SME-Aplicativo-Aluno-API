using MediatR;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioNotificacao
{
    public class ObterUsuarioNotificacaoQuery : IRequest<UsuarioNotificacao>
    {
        public long UsuarioId { get; set; }
        public long CodigoAluno { get; set; }
        public long NotificacaoId { get; set; }
    }
}
