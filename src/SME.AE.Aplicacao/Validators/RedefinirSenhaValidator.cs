using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class RedefinirSenhaValidator : AbstractValidator<RedefinirSenhaDto>
    {
        public RedefinirSenhaValidator()
        {
            RuleFor(x => x.Senha).NotEmpty().WithMessage("É Obrigátorio informar a senha");

            RuleFor(x => x.Senha).ValidarSenha().When(x => !string.IsNullOrWhiteSpace(x.Senha));

            RuleFor(x => x.Token).NotEmpty().WithMessage("É Obrigátorio informar o token");

            RuleFor(x => x.Token).Length(8).WithMessage("O Tamanho do token esta invalido").When(x => !string.IsNullOrWhiteSpace(x.Token));
        }
    }
}
