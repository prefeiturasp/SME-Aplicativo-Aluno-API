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
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é obrigátorio").ValidarCpf();
            RuleFor(x => x.DataNascimento).NotEmpty().WithMessage("A Data de Nascimento é obrigátoria").DataNascimentoEhValida();
        }
    }
}
