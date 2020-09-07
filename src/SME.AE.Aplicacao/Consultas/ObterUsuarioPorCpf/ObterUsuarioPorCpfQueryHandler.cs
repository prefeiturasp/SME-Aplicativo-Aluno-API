using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioPorCpfQueryHandler : IRequestHandler<ObterUsuarioPorCpfQuery, Usuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterUsuarioPorCpfQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new System.ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorCpfQuery request, CancellationToken cancellationToken)
        {
            return await usuarioRepository.ObterPorCpf(request.Cpf);
        }
    }
}
