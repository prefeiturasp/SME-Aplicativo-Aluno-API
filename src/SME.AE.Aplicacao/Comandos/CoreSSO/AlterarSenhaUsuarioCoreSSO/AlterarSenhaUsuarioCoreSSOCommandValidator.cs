using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO
{
    public class AlterarSenhaUsuarioCoreSSOCommandValidator : AbstractValidator<AlterarSenhaUsuarioCoreSSOCommand>
    {
        public AlterarSenhaUsuarioCoreSSOCommandValidator()
        {
            RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("O Id do Usuário é Obrigátorio");
            RuleFor(x => x.SenhaCriptograda).NotEmpty().WithMessage("A Senha é Obrigátoria");
        }
    }
}
