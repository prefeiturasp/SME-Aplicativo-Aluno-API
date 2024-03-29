﻿using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario
{
    public class SalvarUsuarioCommandValidator : AbstractValidator<SalvarUsuarioCommand>
    {
        public SalvarUsuarioCommandValidator()
        {
            RuleFor(x => x.Usuario).NotNull().WithMessage("È necessário informar um usuário");
        }
    }
}
