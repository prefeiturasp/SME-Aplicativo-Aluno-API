using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterUltimaAtualizacaoPorProcessoQueryValidator : AbstractValidator<ObterUltimaAtualizacaoPorProcessoQuery>
    {
        public ObterUltimaAtualizacaoPorProcessoQueryValidator()
        {
            RuleFor(x => x.NomeProcesso).NotEmpty().WithMessage("O nome do processo é obrigatório");
        }
    }
}
