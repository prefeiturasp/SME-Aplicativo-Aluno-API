using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterUsuarioPorCpfQueryValidator : AbstractValidator<ObterUsuarioNaoExcluidoPorCpfQuery>
    {
        public ObterUsuarioPorCpfQueryValidator()
        {
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
