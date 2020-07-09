using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO
{
    public class AlterarSenhaUsuarioCoreSSOCommand : IRequest
    {
        public AlterarSenhaUsuarioCoreSSOCommand(Guid usuarioId, string senhaCriptograda)
        {
            UsuarioId = usuarioId;
            SenhaCriptograda = senhaCriptograda;
        }

        public Guid UsuarioId { get; set; }
        public string SenhaCriptograda { get; set; }
    }
}
