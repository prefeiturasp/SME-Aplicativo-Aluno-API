using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO
{
    public class ObterUsuarioCoreSSOQueryHandler : IRequestHandler<ObterUsuarioCoreSSOQuery, RetornoUsuarioCoreSSO>
    {
        private readonly IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio;
        private readonly IUsuarioGrupoRepositorio usuarioGrupoRepositorio;

        public ObterUsuarioCoreSSOQueryHandler(IUsuarioCoreSSORepositorio usuarioCoreSSORepositorio, IUsuarioGrupoRepositorio usuarioGrupoRepositorio)
        {
            this.usuarioCoreSSORepositorio = usuarioCoreSSORepositorio ?? throw new ArgumentNullException(nameof(usuarioCoreSSORepositorio));
            this.usuarioGrupoRepositorio = usuarioGrupoRepositorio ?? throw new ArgumentNullException(nameof(usuarioGrupoRepositorio));
        }

        public async Task<RetornoUsuarioCoreSSO> Handle(ObterUsuarioCoreSSOQuery request, CancellationToken cancellationToken)
        {
            RetornoUsuarioCoreSSO retorno = null;

            if (!string.IsNullOrWhiteSpace(request.Cpf))
                retorno = await usuarioCoreSSORepositorio.ObterPorCPF(request.Cpf);
            else
                retorno = await usuarioCoreSSORepositorio.ObterPorId(request.UsuarioId);

            if (retorno == null)
                return retorno;

            var grupos = await usuarioGrupoRepositorio.ObterPorUsuarioId(retorno.UsuId);

            retorno.Grupos = grupos?.Select(x => x.GrupoId) ?? default;

            return retorno;
        }
    }
}
