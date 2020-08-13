using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterUsuarioPorTokenRedefinicaoQueryHandler : IRequestHandler<ObterUsuarioPorTokenRedefinicaoQuery, Usuario>
    {
        private readonly IUsuarioRepository usuarioRepository;

        public ObterUsuarioPorTokenRedefinicaoQueryHandler(IUsuarioRepository usuarioRepository)
        {
            this.usuarioRepository = usuarioRepository ?? throw new ArgumentNullException(nameof(usuarioRepository));
        }

        public async Task<Usuario> Handle(ObterUsuarioPorTokenRedefinicaoQuery request, CancellationToken cancellationToken)
        {
            var retorno = await usuarioRepository.ObterUsuarioPorTokenAutenticacao(request.Token);

            if (retorno == null)
                throw new NegocioException("Codigo de Verificação inválido");

            return retorno;
        }
    }
}
