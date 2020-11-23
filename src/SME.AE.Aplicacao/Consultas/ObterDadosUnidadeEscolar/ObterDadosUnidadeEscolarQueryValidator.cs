using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.UnidadeEscolar
{
    public class ObterDadosUnidadeEscolarQueryValidator : AbstractValidator<ObterDadosUnidadeEscolarQuery>
    {
        public ObterDadosUnidadeEscolarQueryValidator()
        {
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O código da Unidade Escolar (UE) é obrigatório");
        }
    }
}
