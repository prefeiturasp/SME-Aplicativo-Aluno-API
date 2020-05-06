using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoUsuarioCommand : IRequest<bool>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoUsuarioCommand(long[] ids)
        {
            Ids = ids;
        }
    }

    public class RemoverNotificacaoUsuarioCommandHandler : IRequestHandler<RemoverNotificacaoUsuarioCommand, bool>
    {
        private readonly IUsuarioNotificacaoRepositorio _repository;

        public RemoverNotificacaoUsuarioCommandHandler(IUsuarioNotificacaoRepositorio repository)
        {
            _repository = repository;
        }


        public async Task<bool> Handle(RemoverNotificacaoUsuarioCommand request, CancellationToken cancellationToken)
        {
            for (int i = 0; i < request.Ids.Length; i++)
            {
                bool retorno = await _repository.Remover(request.Ids[i]);

               if (!retorno)
                    return false;
            }

            return true;

        }
    }
}
