using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Notificacao.Criar
{
    public class CriarNotificacaoCommandValidator : AbstractValidator<CriarNotificacaoCommand>
    {
        public CriarNotificacaoCommandValidator()
        {
            RuleFor(x => x.Notificacao.Id).NotEmpty().WithMessage("O ID da notificação é obrigatório");
            RuleFor(x => x.Notificacao.DataEnvio).NotEmpty().WithMessage("A Data de envio da notificação é obrigatória");
            RuleFor(x => x.Notificacao.DataExpiracao).NotEmpty().WithMessage("A Data de expiração da notificação é obrigatória");
            RuleFor(x => x.Notificacao.Titulo).NotEmpty().WithMessage("O Título da notificação é obrigatório");
            RuleFor(x => x.Notificacao.Mensagem).NotEmpty().WithMessage("A Mensagem da notificação é obrigatória");
        }
    }
}
