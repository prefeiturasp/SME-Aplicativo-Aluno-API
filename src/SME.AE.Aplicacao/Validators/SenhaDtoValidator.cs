using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class SenhaDtoValidator : AbstractValidator<SenhaDto>
    {
        public SenhaDtoValidator()
        {
            RuleFor(x => x.Senha).NotEmpty().WithMessage("È necessario informar a senha");
            RuleFor(x => x.Senha).ValidarSenha().When(x => !string.IsNullOrWhiteSpace(x.Senha));
        }
    }
}
