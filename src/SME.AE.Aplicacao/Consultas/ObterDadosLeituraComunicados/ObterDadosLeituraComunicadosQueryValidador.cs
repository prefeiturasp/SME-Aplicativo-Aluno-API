using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosQueryValidator : AbstractValidator<ObterDadosLeituraComunicadosQuery>
    {
        public ObterDadosLeituraComunicadosQueryValidator()
        {
            RuleFor(x => x.NotificaoId).NotEmpty().WithMessage("O ID da notificação é obrigatório");
            RuleFor(x => x.ModoVisualizacao).NotEmpty().WithMessage("O Modo de visualizaçõ é obrigatório");

        }
    }
}
