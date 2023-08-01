using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoPorIdCommandHandler : IRequestHandler<RemoverNotificacaoPorIdCommand, bool>
    {
        private readonly INotificacaoRepositorio _repository;

        public RemoverNotificacaoPorIdCommandHandler(INotificacaoRepositorio repository)
        {
            _repository = repository;
        }


        public async Task<bool> Handle(RemoverNotificacaoPorIdCommand request, CancellationToken cancellationToken)
        {
            var notificacao = await _repository.ObterPorIdAsync(request.Id);

            if (notificacao == null)
                return false;

            try
            {
                await _repository.Remover(notificacao);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
