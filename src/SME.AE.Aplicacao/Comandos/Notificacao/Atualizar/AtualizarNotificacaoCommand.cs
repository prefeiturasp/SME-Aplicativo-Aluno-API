using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;

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
            var resultadoAtualizacao = await _repository.Atualizar(request.Notificacao);
            if (resultadoAtualizacao == null)
                throw new NegocioException("Não foi possível atualizar o comunicado na base do Escola Aqui!");
            return resultadoAtualizacao;
        }
    }
}