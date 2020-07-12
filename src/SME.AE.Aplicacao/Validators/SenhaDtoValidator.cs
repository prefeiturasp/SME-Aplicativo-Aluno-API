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
            RuleFor(x => x.NovaSenha).NotEmpty().WithMessage("È necessario informar a nova senha");
            RuleFor(x => x.NovaSenha).ValidarSenha().When(x => !string.IsNullOrWhiteSpace(x.NovaSenha));

            RuleFor(x => x.SenhaAntiga).NotEmpty().WithMessage("È necessario informar a senha antiga");
        }
    }
}
