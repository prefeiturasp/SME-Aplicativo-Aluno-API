using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Atualizar
{
    public class AtualizarNotificacaoCommand : IRequest<Dominio.Entidades.Notificacao>
    {
        public Dominio.Entidades.Notificacao Notificacao { get; set; }

        public AtualizarNotificacaoCommand(Dominio.Entidades.Notificacao notificacao)
        {
            Notificacao = notificacao;
        }
    }

    public class AtualizarNotificacaoCommandHandler : IRequestHandler<AtualizarNotificacaoCommand, 
        Dominio.Entidades.Notificacao>
    {
        private readonly INotificacaoRepository _repository;
    
        public AtualizarNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }

        public async Task<Dominio.Entidades.Notificacao> Handle
            (AtualizarNotificacaoCommand request, CancellationToken cancellationToken)
        {
            return await _repository.Atualizar(request.Notificacao);
        }
    }
}