using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario
{
    public class AssociarGrupoUsuarioCommandHandler : IRequestHandler<AssociarGrupoUsuarioCommand>
    {
        private readonly IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio;

        public AssociarGrupoUsuarioCommandHandler(IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio)
        {
            this.usuarioCoreSSORepositorio = usuarioCoreSSORepositorio ?? throw new ArgumentNullException(nameof(usuarioCoreSSORepositorio));
        }

        public async Task<Unit> Handle(AssociarGrupoUsuarioCommand request, CancellationToken cancellationToken)
        {
            var grupos = await usuarioCoreSSORepositorio.SelecionarGrupos();

            if (grupos == null)
                throw new NegocioException("Grupos de usuário não encontrados");

            var gruposNaoIncluidos = grupos.Where(x => !request.UsuarioCoreSSO.Grupos.Any(z => z.Equals(x)));

            if (gruposNaoIncluidos.Any())
                await usuarioCoreSSORepositorio.IncluirUsuarioNosGrupos(request.UsuarioCoreSSO.UsuId, gruposNaoIncluidos);

            return default;
        }
    }
}
