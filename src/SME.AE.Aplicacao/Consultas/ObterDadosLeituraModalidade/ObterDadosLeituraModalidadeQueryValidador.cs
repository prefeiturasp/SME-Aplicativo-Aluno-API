using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraModalidadeQueryValidator : AbstractValidator<ObterDadosLeituraModalidadeQuery>
    {
        public ObterDadosLeituraModalidadeQueryValidator()
        {
            RuleFor(x => x.CodigoDre).NotEmpty().WithMessage("O código DRE é obrigatório");
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O código UE é obrigatório");
            RuleFor(x => x.NotificaoId).NotEmpty().WithMessage("O ID da notificação é obrigatório");
            RuleFor(x => x.ModoVisualizacao).NotEmpty().WithMessage("O Modo de visualização é obrigatório");
        }
    }
}
