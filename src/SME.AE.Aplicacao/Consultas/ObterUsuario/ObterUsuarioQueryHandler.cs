using System;
using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioQueryHandler : IRequestHandler<ObterUsuarioQuery, Usuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterUsuarioQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new System.ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioQuery request, CancellationToken cancellationToken)
        {

            try
            {
                Usuario usuario = default;

                if (!string.IsNullOrWhiteSpace(request.Cpf))
                    usuario = await usuarioRepository.ObterUsuarioNaoExcluidoPorCpf(request.Cpf);
                else
                    usuario = await usuarioRepository.ObterPorIdAsync(request.Id);

                return usuario ?? throw new NegocioException($"Usuário não encontrado");
            }
            catch (Exception e)
            {
                throw new NegocioException(e.Message);
            }
        }
    }
}
