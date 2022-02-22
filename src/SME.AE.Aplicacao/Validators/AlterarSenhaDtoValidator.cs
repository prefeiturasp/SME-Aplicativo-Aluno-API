using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.Validators
{
    public class AlterarSenhaDtoValidator : AbstractValidator<AlterarSenhaDto>
    {
        public AlterarSenhaDtoValidator()
        {
            RuleFor(x => x.Senha).NotEmpty().WithMessage("A senha deve ser informada");
            RuleFor(x => x.Senha).ValidarSenha().When(x => !string.IsNullOrWhiteSpace(x.Senha));
        }
    }
}
