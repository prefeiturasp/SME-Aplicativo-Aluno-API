using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Atualizar
{
    public class AtualizarNotificacaoCommandValidator : AbstractValidator<AtualizarNotificacaoCommand>
    {
        public AtualizarNotificacaoCommandValidator()
        {
            RuleFor(c => c.Titulo)
                .NotEmpty()
                    .WithMessage("O título da notificação deve ser informado!");
            RuleFor(c => c.Mensagem)
                .NotEmpty()
                    .WithMessage("A mensagem da notificação deve ser informada!");

            RuleFor(c => c.DataExpiracao)
                .NotEmpty()
                    .WithMessage("A data de expiração deve ser informada!");

            RuleFor(c => c.AlteradoPor)
                .NotEmpty()
                    .WithMessage("O campo Alterado Por deve ser informada!");
        }
    }
}