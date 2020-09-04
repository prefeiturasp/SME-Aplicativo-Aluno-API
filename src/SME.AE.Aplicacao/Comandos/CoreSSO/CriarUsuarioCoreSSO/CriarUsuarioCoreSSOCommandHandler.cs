using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Comum.Excecoes;
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
            var usuarios = await repositoryCoreSSO.ObterPorCPF(request.Usuario.Cpf);

            if (usuarios != null)
                throw new NegocioException($"Já existe usuário com o CPF {request.Usuario.Cpf} na base do CoreSSO");
           
            var usuarioId = await repositoryCoreSSO.Criar(request.Usuario);

            var grupos = await repositoryCoreSSO.SelecionarGrupos();

            await repositoryCoreSSO.IncluirUsuarioNosGrupos(usuarioId, grupos);

            return await repositoryCoreSSO.ObterPorId(usuarioId);
        }
    }
}
