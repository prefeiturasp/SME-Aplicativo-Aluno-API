using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos
{
    public class ObterTotalUsuariosValidosQueryHandler : IRequestHandler<ObterTotalUsuariosValidosQuery, long>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterTotalUsuariosValidosQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new System.ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<long> Handle(ObterTotalUsuariosValidosQuery request, CancellationToken cancellationToken)
        {
            return await usuarioRepository.ObterTotalUsuariosValidos(request.Cpfs);
        }
    }
}
