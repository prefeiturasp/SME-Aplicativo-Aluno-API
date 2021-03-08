using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterStatusNotificacaoUsuarioQueryValidator : AbstractValidator<ObterStatusNotificacaoUsuarioQuery>
    {
        public ObterStatusNotificacaoUsuarioQueryValidator()
        {
            RuleFor(x => x.NotificoesId).NotNull().WithMessage("O ID da notificação é obrigatório");
            RuleFor(x => x.CodigoAluno).NotEmpty().WithMessage("O código da turma é obrigatório");
        }
    }
}
