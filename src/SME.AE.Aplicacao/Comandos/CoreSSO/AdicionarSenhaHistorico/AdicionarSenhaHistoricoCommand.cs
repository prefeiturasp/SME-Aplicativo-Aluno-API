using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

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
