using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioNotificacao
{
    public class ObterUsuarioNotificacaoQueryHandler : IRequestHandler<ObterUsuarioNotificacaoQuery, UsuarioNotificacao>
    {
        private readonly IUsuarioNotificacaoRepositorio usuarioNotificacaoRepositorio;

        public ObterUsuarioNotificacaoQueryHandler(IUsuarioNotificacaoRepositorio usuarioNotificacaoRepositorio)
        {
            this.usuarioNotificacaoRepositorio = usuarioNotificacaoRepositorio ?? throw new System.ArgumentNullException(nameof(usuarioNotificacaoRepositorio));
        }

        public async Task<UsuarioNotificacao> Handle(ObterUsuarioNotificacaoQuery request, CancellationToken cancellationToken)
        {
            return await usuarioNotificacaoRepositorio.ObterPorUsuarioAlunoNotificacao(request.UsuarioId, request.CodigoAluno, request.NotificacaoId);
        }
    }
}
