using Elastic.Apm;
using Elastic.Apm.Api;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Logs
{
    public class SalvarLogErroCommandHandler : IRequestHandler<SalvarLogErroCommand, bool>
    {
        public Task<bool> Handle(SalvarLogErroCommand request, CancellationToken cancellationToken)
        {
            var transacao = Agent.Tracer.CurrentTransaction;

            if (transacao != null)
            {
                foreach (var label in request.Labels)
                {
                    transacao.SetLabel(label.Key, label.Value);
                }
                if (!string.IsNullOrWhiteSpace(request.MensagemCustomizada))
                {
                    transacao.CaptureErrorLog(new ErrorLog(request.MensagemCustomizada));
                }
                if (request.Excecao != null)
                {
                    transacao.CaptureException(request.Excecao);
                }
            }
            return Task.FromResult(true);
        }
    }
}
