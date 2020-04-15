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
    public class RemoverNotificacaoCommand : IRequest<string>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoCommand(long[] ids)
        {
            Ids = ids;
        }
    }

    public class RemoverNotificacaoCommandHandler : IRequestHandler<RemoverNotificacaoCommand, string>
    {
        private readonly INotificacaoRepository _repository;
        
        public RemoverNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }


        public async Task<string> Handle(RemoverNotificacaoCommand request, CancellationToken cancellationToken)
        {
            var erros = new StringBuilder();
            foreach (long id in request.Ids)
            {
                var notificacao =  await _repository.ObterPorId(id);
                if (notificacao == null)
                {
                    erros.Append($"<li>{id} - comunicado não encontrado</li>");
                }
                    
                else
                {
                    try
                    {
                        await _repository.Remover(notificacao);
                    }
                    catch
                    {
                        erros.Append($"<li>{id} - {notificacao.Titulo}</li>");
                    }
                }
            }
            return erros.ToString();
        }
    }
}