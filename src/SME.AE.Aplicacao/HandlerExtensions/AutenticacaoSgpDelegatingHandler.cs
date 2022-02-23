using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.HandlerExtensions
{
    public class AutenticacaoSgpDelegatingHandler : DelegatingHandler
    {
        private readonly IMediator mediator;

        public AutenticacaoSgpDelegatingHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await mediator.Send(new ObterSgpTokenQuery());

            request.Headers.Clear();
            request.Headers.Add("Authorization", $"Bearer {token}");
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
