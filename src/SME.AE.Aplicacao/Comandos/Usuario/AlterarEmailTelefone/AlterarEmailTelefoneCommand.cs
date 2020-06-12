using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarEmailTelefone
{
    public class AlterarEmailTelefoneCommand : IRequest
    {
        public AlterarEmailTelefoneDto alterarEmailTelefoneDto { get; set; }
    }
}
