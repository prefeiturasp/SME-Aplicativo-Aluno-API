using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoUsuarioCommandHandler : IRequestHandler<RemoverNotificacaoUsuarioCommand, bool>
    {
        private readonly IUsuarioNotificacaoRepositorio _repository;

        public RemoverNotificacaoUsuarioCommandHandler(IUsuarioNotificacaoRepositorio repository)
        {
            _repository = repository;
        }


        public async Task<bool> Handle(RemoverNotificacaoUsuarioCommand request, CancellationToken cancellationToken)
        {
            return await _repository.RemoverPorNotificacoesIds(request.Ids);
        }
    }
}
