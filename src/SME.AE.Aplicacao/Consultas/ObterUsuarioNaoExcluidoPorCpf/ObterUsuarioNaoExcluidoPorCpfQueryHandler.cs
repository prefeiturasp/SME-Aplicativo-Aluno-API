using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioNaoExcluidoPorCpfQueryHandler : IRequestHandler<ObterUsuarioNaoExcluidoPorCpfQuery, Usuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterUsuarioNaoExcluidoPorCpfQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new System.ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioNaoExcluidoPorCpfQuery request, CancellationToken cancellationToken)
        {
            return await usuarioRepository.ObterUsuarioNaoExcluidoPorCpf(request.Cpf);
        }
    }
}
