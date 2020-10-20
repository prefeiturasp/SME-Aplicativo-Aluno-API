using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Consultas.ObterTotalUsuariosComAcessoIncompleto;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterTotalUsuariosComAcessoIncompletoQueryHandler : IRequestHandler<ObterTotalUsuariosComAcessoIncompletoQuery, long>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterTotalUsuariosComAcessoIncompletoQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new System.ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<long> Handle(ObterTotalUsuariosComAcessoIncompletoQuery request, CancellationToken cancellationToken)
        {
            return await usuarioRepository.ObterTotalUsuariosComAcessoIncompleto(request.Cpfs);
        }
    }
}
