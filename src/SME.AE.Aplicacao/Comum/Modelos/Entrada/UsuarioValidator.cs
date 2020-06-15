using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Cpf).NotNull().NotEmpty().ValidarCpf();
            RuleFor(x => x.DataNascimento).NotNull().NotEmpty().DataNascimentoEhValida();
        }
    }
}
