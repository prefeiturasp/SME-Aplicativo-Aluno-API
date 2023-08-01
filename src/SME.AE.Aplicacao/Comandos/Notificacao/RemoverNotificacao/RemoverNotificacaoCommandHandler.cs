using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Remover
{
    public class RemoverNotificacaoCommandHandler : IRequestHandler<RemoverNotificacaoCommand, string[]>
    {
        private readonly INotificacaoRepositorio _repository;

        public RemoverNotificacaoCommandHandler(INotificacaoRepositorio repository)
        {
            _repository = repository;
        }


        public async Task<string[]> Handle(RemoverNotificacaoCommand request, CancellationToken cancellationToken)
        {
            var erros = new StringBuilder();
            string[] errosArray = new string[request.Ids.Length];

            for (int i = 0; i < request.Ids.Length; i++)
            {
                var notificacao = await _repository.ObterPorIdAsync(request.Ids[i]);
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