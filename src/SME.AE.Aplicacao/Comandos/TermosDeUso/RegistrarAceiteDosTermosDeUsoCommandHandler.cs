using MediatR;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario
{
    public class RegistrarAceiteDosTermosDeUsoCommandHandler : IRequestHandler<RegistrarAceiteDosTermosDeUsoCommand, bool>
    {
        private readonly IAceiteTermosDeUsoRepositorio _aceiteTermosDeUsorepositorio;

        public RegistrarAceiteDosTermosDeUsoCommandHandler(IAceiteTermosDeUsoRepositorio aceiteTermosDeUsorepositorio)
        {
            _aceiteTermosDeUsorepositorio = aceiteTermosDeUsorepositorio ?? throw new System.ArgumentNullException(nameof(aceiteTermosDeUsorepositorio));
        }

        public async Task<bool> Handle(RegistrarAceiteDosTermosDeUsoCommand request, CancellationToken cancellationToken)
        {
            return await _aceiteTermosDeUsorepositorio.RegistrarAceite(new Dominio.Entidades.AceiteTermosDeUso(request.TermoDeUsoId, request.Usuario, request.Device, request.Ip, request.Versao));
        }
    }
}
