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
    public class RemoverNotificacaoCommand : IRequest<string[]>
    {
        public long[] Ids { get; set; }

        public RemoverNotificacaoCommand(long[] ids)
        {
            Ids = ids;
        }
    }

    public class RemoverNotificacaoCommandHandler : IRequestHandler<RemoverNotificacaoCommand, string[]>
    {
        private readonly INotificacaoRepository _repository;
        
        public RemoverNotificacaoCommandHandler(INotificacaoRepository repository)
        {
            _repository = repository;
        }


        public async Task<string[]> Handle(RemoverNotificacaoCommand request, CancellationToken cancellationToken)
        {
            var erros = new StringBuilder();
            string[] errosArray = new string[request.Ids.Length];
          
            for (int i = 0;  i < request.Ids.Length; i++)
            {
                var notificacao =  await _repository.ObterPorIdAsync(request.Ids[i]);
                if (notificacao == null)
                {
                    errosArray.SetValue($"{request.Ids[i]} - Notificação não encontrada", i);
                }
                    
                else
                {
                    try
                    {
                        await _repository.Remover(notificacao);
                    }
                    catch
                    {
                        erros.Append($"<li>{request.Ids[i]} - {notificacao.Titulo}</li>");
                    }
                }
            }
            return errosArray;
        }
    }
}