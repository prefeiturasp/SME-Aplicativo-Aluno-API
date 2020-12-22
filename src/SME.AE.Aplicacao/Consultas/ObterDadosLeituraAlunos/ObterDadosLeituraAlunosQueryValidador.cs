using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraAlunosQueryValidator : AbstractValidator<ObterDadosLeituraAlunosQuery>
    {
        public ObterDadosLeituraAlunosQueryValidator()
        {
            RuleFor(x => x.NotificaoId).NotEmpty().WithMessage("O ID da notificação é obrigatório");
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("O código da turma é obrigatório");
        }
    }
}
