using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.TesteArquitetura
{
    class TesteArquiteturaCommandHandler : IRequestHandler<TesteArquiteturaCommand>
    {
        public async Task<Unit> Handle(TesteArquiteturaCommand request, CancellationToken cancellationToken)
        {
            return default;
        }
    }
}
