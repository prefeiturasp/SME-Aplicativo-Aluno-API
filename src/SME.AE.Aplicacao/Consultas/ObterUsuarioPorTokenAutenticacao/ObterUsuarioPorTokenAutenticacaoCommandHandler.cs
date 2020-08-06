using MediatR;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenAutenticacao
{
    public class ObterUsuarioPorTokenAutenticacaoCommandHandler : IRequestHandler<ObterUsuarioPorTokenAutenticacaoCommand, Usuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterUsuarioPorTokenAutenticacaoCommandHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorTokenAutenticacaoCommand request, CancellationToken cancellationToken)
        {
            var retorno = await usuarioRepository.ObterUsuarioPorTokenAutenticacao(request.Token);

            if (retorno == null)
                throw new NegocioException("Token invalido");

            return retorno;
        }
    }
}
