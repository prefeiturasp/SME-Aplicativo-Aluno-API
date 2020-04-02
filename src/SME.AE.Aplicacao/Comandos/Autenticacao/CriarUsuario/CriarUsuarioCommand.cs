using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario
{
    public class CriarUsuarioCommand : IRequest<RespostaAutenticar>
    {
    }

    public class CriarUsuarioCommandCommandHandler : IRequestHandler<CriarUsuarioCommand, RespostaAutenticar>
    {
        private readonly IAplicacaoContext _context;
        private readonly IAutenticacaoService _autenticacaoService;

        public CriarUsuarioCommandCommandHandler(IAplicacaoContext context, IAutenticacaoService autenticacaoService)
        {
            _context = context;
            _autenticacaoService = autenticacaoService;
        }

        public Task<RespostaAutenticar> Handle(CriarUsuarioCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
