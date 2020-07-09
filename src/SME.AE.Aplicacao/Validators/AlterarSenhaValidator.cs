using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos;

namespace SME.AE.Aplicacao.Validators
{
    public class AlterarSenhaValidator : AbstractValidator<AlterarSenhaDto>
    {
        public AlterarSenhaValidator()
        {
            RuleFor(x => x.CPF).NotEmpty().WithMessage("Deve ser informado o CPF");
            RuleFor(x => x.CPF).ValidarCpf().When(x => !string.IsNullOrWhiteSpace(x.CPF));

            RuleFor(x => x.Senha).NotEmpty().WithMessage("Deve ser informado a Senha");
            RuleFor(x => x.Senha).ValidarSenha().When(x => !string.IsNullOrWhiteSpace(x.Senha));
        }
    }
}
