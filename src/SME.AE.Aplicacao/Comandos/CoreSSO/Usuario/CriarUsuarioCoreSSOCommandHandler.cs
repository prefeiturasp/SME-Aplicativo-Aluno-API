using MediatR;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.Usuario
{
    public class CriarUsuarioCoreSSOCommandHandler : IRequestHandler<CriarUsuarioCoreSSOCommand, RetornoUsuarioCoreSSO>
    {
        private readonly IUsuarioCoreSSORepositorio repositoryCoreSSO;

        public CriarUsuarioCoreSSOCommandHandler(IUsuarioCoreSSORepositorio repositoryCoreSSO)
        {
            this.repositoryCoreSSO = repositoryCoreSSO ?? throw new ArgumentNullException(nameof(repositoryCoreSSO));
        }

        public async Task<RetornoUsuarioCoreSSO> Handle(CriarUsuarioCoreSSOCommand request, CancellationToken cancellationToken)
        {
            var usuarios = await repositoryCoreSSO.Selecionar(request.Usuario.Cpf);

            if (usuarios.Any())
                throw new NegocioException($"Já existe usuário com o CPF {request.Usuario.Cpf} na base do CoreSSO");

            var usuarioId = await repositoryCoreSSO.Criar(request.Usuario);

            return await repositoryCoreSSO.ObterPorId(usuarioId);
        }
    }
}
