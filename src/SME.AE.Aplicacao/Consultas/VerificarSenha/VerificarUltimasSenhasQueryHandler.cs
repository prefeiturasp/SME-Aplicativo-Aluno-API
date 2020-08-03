using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.VerificarSenha
{
    public class VerificarUltimasSenhasQueryHandler : IRequestHandler<VerificarUltimasSenhasQuery, bool>
    {
        private readonly IUsuarioSenhaHistoricoCoreSSORepositorio usuarioSenhaHistoricoCoreSSORepositorio;

        public VerificarUltimasSenhasQueryHandler(IUsuarioSenhaHistoricoCoreSSORepositorio usuarioSenhaHistoricoCoreSSORepositorio)
        {
            this.usuarioSenhaHistoricoCoreSSORepositorio = usuarioSenhaHistoricoCoreSSORepositorio ?? throw new System.ArgumentNullException(nameof(usuarioSenhaHistoricoCoreSSORepositorio));
        }

        public async Task<bool> Handle(VerificarUltimasSenhasQuery request, CancellationToken cancellationToken)
        {
            return await usuarioSenhaHistoricoCoreSSORepositorio.VerificarUltimas5Senhas(request.UsuarioIdCore, request.SenhaCriptografada);
        }
    }
}
