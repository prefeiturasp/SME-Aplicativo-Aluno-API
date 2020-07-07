using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico
{
    public class AdicionarSenhaHistoricoCommandHandler : IRequestHandler<AdicionarSenhaHistoricoCommand>
    {
        private readonly IUsuarioSenhaHistoricoCoreSSORepositorio usuarioSenhaHistoricoCoreSSORepositorio;

        public AdicionarSenhaHistoricoCommandHandler(IUsuarioSenhaHistoricoCoreSSORepositorio usuarioSenhaHistoricoCoreSSORepositorio)
        {
            this.usuarioSenhaHistoricoCoreSSORepositorio = usuarioSenhaHistoricoCoreSSORepositorio ?? throw new ArgumentNullException(nameof(usuarioSenhaHistoricoCoreSSORepositorio));
        }

        public async Task<Unit> Handle(AdicionarSenhaHistoricoCommand request, CancellationToken cancellationToken)
        {
            var historico = new UsuarioSenhaHistoricoCoreSSO
            {
                Criptografia = Dominio.Comum.Enumeradores.TipoCriptografia.TripleDES,
                Data = DateTime.Now,
                Senha = request.SenhaCriptografada,
                UsuarioId = request.UsuarioId
            };

            await usuarioSenhaHistoricoCoreSSORepositorio.AdicionarSenhaHistorico(historico);

            return default;
        }
    }
}
