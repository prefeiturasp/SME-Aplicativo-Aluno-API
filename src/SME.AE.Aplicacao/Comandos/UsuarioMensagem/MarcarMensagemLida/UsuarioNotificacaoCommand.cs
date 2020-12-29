using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.MarcarMensagemLida
{
    public class UsuarioNotificacaoCommand : IRequest<bool>
    {
        public UsuarioNotificacaoCommand(UsuarioNotificacao usuarioNotificacao)
        {
            UsuarioNotificacao = usuarioNotificacao;
        }
        private UsuarioNotificacao UsuarioNotificacao { get; set; }

        public class UsuarioMensagemCommandHandler : IRequestHandler<UsuarioNotificacaoCommand, bool>
        {
            private readonly IUsuarioNotificacaoRepositorio _repository;


            public UsuarioMensagemCommandHandler(IUsuarioNotificacaoRepositorio repository)
            {
                _repository = repository;

            }

            public async Task<bool> Handle(UsuarioNotificacaoCommand request, CancellationToken cancellationToken)
            {
                var usuarioNotificacaoRequest = request.UsuarioNotificacao;

                var usuarioNotificacao = await _repository.ObterPorNotificacaoIdEhUsuarioCpf(usuarioNotificacaoRequest.NotificacaoId,
                                                                                             usuarioNotificacaoRequest.UsuarioCpf,
                                                                                             usuarioNotificacaoRequest.DreCodigoEol,
                                                                                             usuarioNotificacaoRequest.UeCodigoEol,
                                                                                             usuarioNotificacaoRequest.CodigoEolAluno);

                if (usuarioNotificacao == null)
                    return await _repository.Criar(request.UsuarioNotificacao);

                usuarioNotificacao.MensagemVisualizada = request.UsuarioNotificacao.MensagemVisualizada;

                usuarioNotificacao.AtualizarAuditoria();

                return await _repository.Atualizar(usuarioNotificacao);
            }
        }
    }
}
