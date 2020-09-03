using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQueryValidator : AbstractValidator<ObterDadosAlunosQuery>
    {
        public ObterDadosAlunosQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
