using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterParametrosSistemaPorChavesQueryValidator : AbstractValidator<ObterParametrosSistemaPorChavesQuery>
    {
        public ObterParametrosSistemaPorChavesQueryValidator()
        {
            RuleFor(x => x.Chaves).NotEmpty().WithMessage("As chaves para busca devem ser informadas");
        }
    }
}
