using MediatR;
using SME.AE.Aplicacao.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Text;
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
