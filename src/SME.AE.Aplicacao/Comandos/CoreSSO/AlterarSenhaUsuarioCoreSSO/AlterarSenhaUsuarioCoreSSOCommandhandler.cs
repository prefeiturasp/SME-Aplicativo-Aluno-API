using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO
{
    public class AlterarSenhaUsuarioCoreSSOCommandhandler : IRequestHandler<AlterarSenhaUsuarioCoreSSOCommand>
    {
        private readonly IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio;

        public AlterarSenhaUsuarioCoreSSOCommandhandler(IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio)
        {
            this.usuarioCoreSSORepositorio = usuarioCoreSSORepositorio ?? throw new ArgumentNullException(nameof(usuarioCoreSSORepositorio));
        }

        public async Task<Unit> Handle(AlterarSenhaUsuarioCoreSSOCommand request, CancellationToken cancellationToken)
        {
            await usuarioCoreSSORepositorio.AlterarSenha(request.UsuarioId, request.SenhaCriptograda);

            return default;
        }
    }
}
