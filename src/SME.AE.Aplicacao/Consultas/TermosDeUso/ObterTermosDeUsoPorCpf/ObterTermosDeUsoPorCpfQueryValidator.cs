using FluentValidation;
using SME.AE.Aplicacao.Consultas.TermosDeUso;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterTermosDeUsoPorCpfQueryValidator : AbstractValidator<ObterTermosDeUsoPorCpfQuery>
    {
        public ObterTermosDeUsoPorCpfQueryValidator()
        {
            RuleFor(x => x.CPF).NotEmpty().WithMessage("O CPF do usuário é Obrigátorio");
            RuleFor(x => x.CPF).ValidarCpf().When(x => !string.IsNullOrWhiteSpace(x.CPF));
        }
    }
}
