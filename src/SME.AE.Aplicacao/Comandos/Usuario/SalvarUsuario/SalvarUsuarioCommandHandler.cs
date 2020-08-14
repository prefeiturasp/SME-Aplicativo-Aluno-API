using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario
{
    public class SalvarUsuarioCommandHandler : IRequestHandler<SalvarUsuarioCommand>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public SalvarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Unit> Handle(SalvarUsuarioCommand request, CancellationToken cancellationToken)
        {
            await usuarioRepository.SalvarAsync(request.Usuario);

            return default;
        }
    }
}
