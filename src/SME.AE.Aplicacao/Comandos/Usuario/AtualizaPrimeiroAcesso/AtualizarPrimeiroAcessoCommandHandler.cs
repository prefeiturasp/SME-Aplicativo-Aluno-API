using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso
{
    public class AtualizarPrimeiroAcessoCommandHandler : IRequestHandler<AtualizarPrimeiroAcessoCommand>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public AtualizarPrimeiroAcessoCommandHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Unit> Handle(AtualizarPrimeiroAcessoCommand request, CancellationToken cancellationToken)
        {
            var usuario = await usuarioRepository.ObterPorIdAsync(request.Id);

            if (usuario == null)
                throw new NegocioException($"Não foi encontrado usuário com o id {request.Id}");

            usuario.PrimeiroAcesso = request.PrimeiroAcesso;

            await usuarioRepository.AtualizarPrimeiroAcesso(request.Id, request.PrimeiroAcesso);

            return default;
        }
    }
}
