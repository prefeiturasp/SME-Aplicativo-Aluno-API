using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoPorIdCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public RemoverNotificacaoPorIdCommand(int id)
        {
            Id = id;
        }
    }

    public class RemoverNotificacaoPorIdCommandHandler : IRequestHandler<RemoverNotificacaoPorIdCommand, bool>
    {
        private readonly INotificacaoRepository _repository;

        public RemoverNotificacaoPorIdCommandHandler(INotificacaoRepository repository)
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
