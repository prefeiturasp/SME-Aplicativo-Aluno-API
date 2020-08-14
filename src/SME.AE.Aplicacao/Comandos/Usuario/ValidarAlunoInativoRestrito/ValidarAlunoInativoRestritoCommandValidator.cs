using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Usuario.ValidarAlunoInativoRestrito
{
    public class ValidarAlunoInativoRestritoCommandValidator : AbstractValidator<ValidarAlunoInativoRestritoCommand>
    {
        public ValidarAlunoInativoRestritoCommandValidator()
        {
            RuleFor(x => x.UsuarioCoreSSO).NotNull().WithMessage("É Obrigátorio informar o Usuário para validação");
        }
    }
}
