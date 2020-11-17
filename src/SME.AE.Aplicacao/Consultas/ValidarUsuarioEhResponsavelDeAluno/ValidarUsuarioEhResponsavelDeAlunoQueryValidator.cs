using FluentValidation;

namespace SME.AE.Aplicacao.Consultas
{
    public class ValidarUsuarioEhResponsavelDeAlunoQueryValidator : AbstractValidator<ValidarUsuarioEhResponsavelDeAlunoQuery>
    {
        public ValidarUsuarioEhResponsavelDeAlunoQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é obrigátorio");
        }
    }
}
