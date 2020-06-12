using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular
{
    public class AlterarEmailCelularCommandHandler : IRequestHandler<AlterarEmailCelularCommand>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public AlterarEmailCelularCommandHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Unit> Handle(AlterarEmailCelularCommand request, CancellationToken cancellationToken)
        {
            var usuario = await usuarioRepository.ObterPorIdAsync(request.AlterarEmailCelularDto.Id);

            if (usuario is null)
                throw new Exception($"Não encontrado usuário com id {request.AlterarEmailCelularDto.Id}");

            if (!string.IsNullOrWhiteSpace(request.AlterarEmailCelularDto.Celular))
                usuario.Celular = request.AlterarEmailCelularDto.Celular;

            if (!string.IsNullOrWhiteSpace(request.AlterarEmailCelularDto.Email))
                usuario.Email = request.AlterarEmailCelularDto.Email;

            await usuarioRepository.SalvarAsync(usuario);

            return default;
        }
    }
}
