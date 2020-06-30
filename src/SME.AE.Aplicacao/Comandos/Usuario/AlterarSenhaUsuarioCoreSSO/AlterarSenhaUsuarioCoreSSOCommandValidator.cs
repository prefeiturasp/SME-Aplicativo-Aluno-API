using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.AlterarSenhaUsuarioCoreSSO
{
    public class AlterarSenhaUsuarioCoreSSOCommandValidator : AbstractValidator<AlterarSenhaUsuarioCoreSSOCommand>
    {
        public AlterarSenhaUsuarioCoreSSOCommandValidator()
        {
            RuleFor(x => x.UsuarioId).NotNull().NotEmpty().WithMessage("O Id do Usuário é Obrigátorio");
            RuleFor(x => x.SenhaCriptograda).NotNull().NotEmpty().WithMessage("A Senha é Obrigátoria");
        }
    }
}
