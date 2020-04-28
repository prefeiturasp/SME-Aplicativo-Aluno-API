using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo
{
   public class UsuarioDispositivoCommand : IRequest<bool>
    {
        public UsuarioDispositivoCommand(int usuarioId, string dispositivoId)
        {
            UsuarioId = usuarioId;
            DispositivoId = dispositivoId;
        }
        private int UsuarioId { get; set; }
        private string DispositivoId { get; set; }

        public class UsuarioDispositivoCommandHandler : IRequestHandler<UsuarioDispositivoCommand, bool>
        {
            private readonly IUsuarioRepository _repository;

            public async Task<bool> Handle(UsuarioDispositivoCommand request, CancellationToken cancellationToken)
            {
             
                try
                {
                    // Validar Entradas
                    //Verificar Se usuario Dispositivo ja existe se nao existir criar
                    //Caso exista nao fazer nada.
                    //Veriicar essa arquitetura pois precido mesmo de um response aqui ? 
                    _repository.CriaUsuarioDispositivo(request.UsuarioId, request.DispositivoId);

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
