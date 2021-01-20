using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioNaoExcluidoPorCpfQueryValidator : AbstractValidator<ObterUsuarioNaoExcluidoPorCpfQuery>
    {
        public ObterUsuarioNaoExcluidoPorCpfQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
