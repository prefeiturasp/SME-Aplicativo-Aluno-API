using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioValidator : AbstractValidator<UsuarioDto>
    {
        public UsuarioValidator()
        {
            RuleFor(x => x.Cpf).NotNull().NotEmpty().ValidarCpf().WithMessage("O CPF é obrigátorio");
            RuleFor(x => x.DataNascimento).NotNull().NotEmpty().DataNascimentoEhValida().WithMessage("A Data de Nascimento é obrigátoria");
        }
    }
}
