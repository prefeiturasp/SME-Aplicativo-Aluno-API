using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.VerificarSenha
{
    public class VerificarUltimasSenhasQuery : IRequest<bool>
    {
        public string UsuarioIdCore { get; set; }
        public string SenhaCriptografada { get; set; }
    }
}
