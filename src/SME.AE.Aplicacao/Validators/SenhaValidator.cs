﻿using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;

namespace SME.AE.Aplicacao.Validators
{
    public class SenhaValidator : AbstractValidator<NovaSenhaDto>
    {
        public SenhaValidator()
        {
            RuleFor(x => x.NovaSenha).NotEmpty().WithMessage("A Senha deve ser informada").ValidarSenha();
        }
    }
}
