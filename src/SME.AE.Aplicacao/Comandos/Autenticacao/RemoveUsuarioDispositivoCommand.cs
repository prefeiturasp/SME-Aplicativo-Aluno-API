using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Autenticacao
{
   public class RemoveUsuarioDispositivoCommand : IRequest<bool>
    {
        public RemoveUsuarioDispositivoCommand(string cpf, string dispositivoId)
        {
            Cpf = cpf;
            DispositivoId = dispositivoId;
        }

        public string Cpf { get; set; }
        public string DispositivoId { get; set; }

        public class RemoveUsuarioDispositivoCommandHandler : IRequestHandler<RemoveUsuarioDispositivoCommand, bool>
        {

            private readonly IUsuarioRepository _repository;

            public RemoveUsuarioDispositivoCommandHandler(IUsuarioRepository repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(RemoveUsuarioDispositivoCommand request, CancellationToken cancellationToken)
            {
                var usuarioRetorno = await _repository.ObterUsuarioNaoExcluidoPorCpf(request.Cpf);
                if (usuarioRetorno == null)
                    return false;

                return await _repository.RemoveUsuarioDispositivo(usuarioRetorno.Id, request.DispositivoId);
            }
        }
    }
   
}
