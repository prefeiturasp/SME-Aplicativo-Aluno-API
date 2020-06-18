using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailCelular
{
    public class AlterarEmailCelularCommand : IRequest
    {
        public AlterarEmailCelularCommand(AlterarEmailCelularDto alterarEmailCelularDto)
        {
            AlterarEmailCelularDto = alterarEmailCelularDto;
        }

        public AlterarEmailCelularDto AlterarEmailCelularDto { get; set; }
    }
}
