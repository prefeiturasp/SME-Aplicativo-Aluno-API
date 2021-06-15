using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo
{
    public class UsuarioDispositivoCommand : IRequest<bool>
    {
        public UsuarioDispositivoCommand(string cpfUsuario, string dispositivoId)
        {
            CpfUsuario = cpfUsuario;
            DispositivoId = dispositivoId;
        }
        private string CpfUsuario { get; set; }
        private string DispositivoId { get; set; }

        public class UsuarioDispositivoCommandHandler : IRequestHandler<UsuarioDispositivoCommand, bool>
        {
            private readonly IUsuarioRepository _repository;

            public UsuarioDispositivoCommandHandler(IUsuarioRepository repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(UsuarioDispositivoCommand request, CancellationToken cancellationToken)
            {

                try
                {
                    //Veriicar essa arquitetura pois precido mesmo de um response aqui ? 
                    var usuario = await _repository.ObterUsuarioNaoExcluidoPorCpf(request.CpfUsuario);
                    if (usuario == null)
                        return false;
                    bool existeUsuarioDisposivo = await _repository.ExisteUsuarioDispositivo(usuario.Id, request.DispositivoId);
                    if (!existeUsuarioDisposivo)
                    {
                        await _repository.CriaUsuarioDispositivo(usuario.Id, request.DispositivoId);
                    }

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
