using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosAgrupadosPorDreQueryValidador : AbstractValidator<ObterDadosLeituraComunicadosAgrupadosPorDreQuery>
    {
        public ObterDadosLeituraComunicadosAgrupadosPorDreQueryValidador()
        {
            RuleFor(x => x.NotificaoId).NotEmpty().WithMessage("O ID da notificação é obrigatório");
            RuleFor(x => x.ModoVisualizacao).NotEmpty().WithMessage("O Modo de visualização é obrigatório");
        }
    }
}
