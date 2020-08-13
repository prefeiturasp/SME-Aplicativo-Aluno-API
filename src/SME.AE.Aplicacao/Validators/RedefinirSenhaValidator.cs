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

            RuleFor(x => x.Token).NotEmpty().WithMessage("É Obrigátorio informar o Codigo de Verificação");

            RuleFor(x => x.Token).Length(8).WithMessage("O Tamanho do Codigo de Verificação esta invalido").When(x => !string.IsNullOrWhiteSpace(x.Token));

            RuleFor(x => x.DispositivoId).NotEmpty().WithMessage("O dispositivo Id é Obrigátorio");
        }
    }
}
