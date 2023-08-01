using MediatR;
using System;

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
