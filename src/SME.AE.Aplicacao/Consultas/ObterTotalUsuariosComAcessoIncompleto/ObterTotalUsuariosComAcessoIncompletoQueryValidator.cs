using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosComAcessoIncompleto
{
    public class ObterTotalUsuariosComAcessoIncompletoQueryValidator : AbstractValidator<ObterTotalUsuariosComAcessoIncompletoQuery>
    {
        public ObterTotalUsuariosComAcessoIncompletoQueryValidator()
        {
            RuleFor(x => x.CodigoDre).NotEmpty().WithMessage("O Código da DRE é Obrigátorio");
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O Código da UE é Obrigátorio");
        }
    }
}
