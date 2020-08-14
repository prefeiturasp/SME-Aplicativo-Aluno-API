using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito
{
    public class ValidarAlunoInativoRestritoCommand : IRequest
    {
        public ValidarAlunoInativoRestritoCommand(RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            UsuarioCoreSSO = usuarioCoreSSO;
        }

        public RetornoUsuarioCoreSSO UsuarioCoreSSO { get; set; }

    }
}
