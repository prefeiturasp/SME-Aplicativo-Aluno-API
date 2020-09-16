using FluentValidation;
using SME.AE.Aplicacao.Consultas.TermosDeUso;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterTermosDeUsoQueryValidator : AbstractValidator<ObterTermosDeUsoQuery>
    {
        public ObterTermosDeUsoQueryValidator()
        {
            RuleFor(x => x.CPF).NotEmpty().WithMessage("O CPF do usuário é Obrigátorio");
        }
    }
}
