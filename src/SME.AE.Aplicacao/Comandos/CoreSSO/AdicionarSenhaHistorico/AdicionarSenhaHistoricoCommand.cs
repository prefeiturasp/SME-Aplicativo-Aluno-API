using MediatR;
using System;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico
{
    public class AdicionarSenhaHistoricoCommand : IRequest
    {
        public AdicionarSenhaHistoricoCommand(Guid usuarioId, string senhaCriptografada)
        {
            UsuarioId = usuarioId;
            SenhaCriptografada = senhaCriptografada;
        }

        public Guid UsuarioId { get; set; }
        public string SenhaCriptografada { get; set; }
    }
}
