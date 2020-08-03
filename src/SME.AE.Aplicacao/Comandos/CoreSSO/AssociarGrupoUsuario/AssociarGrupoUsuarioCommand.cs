using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario
{
    public class AssociarGrupoUsuarioCommand : IRequest
    {
        public AssociarGrupoUsuarioCommand(RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            UsuarioCoreSSO = usuarioCoreSSO;
        }

        public RetornoUsuarioCoreSSO UsuarioCoreSSO { get; set; }
    }
}
