using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoCommand : IRequest<bool>
    {
        public Dominio.Entidades.Notificacao Notificacao { get; set; }

        public RemoverNotificacaoCommand(Dominio.Entidades.Notificacao notificacao)
        {
            Notificacao = notificacao;
        }
    }

    public class RemoverNotificacaoCommandHandler : IRequestHandler<RemoverNotificacaoCommand, bool>
    {
        private readonly INotificacaoRepository _repository;
        
        public RemoverNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemoverNotificacaoCommand request, CancellationToken cancellationToken)
        {
            return await _repository.Remover(request.Notificacao);
        }
    }
}