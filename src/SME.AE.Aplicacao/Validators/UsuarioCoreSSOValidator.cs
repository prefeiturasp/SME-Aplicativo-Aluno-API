using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;

namespace SME.AE.Aplicacao.Validators
{
    public class UsuarioCoreSSOValidator : AbstractValidator<UsuarioCoreSSO>
    {
        public UsuarioCoreSSOValidator()
        {
            RuleFor(x => x.Cpf).ValidarCpf();
            RuleFor(x => x.Nome).NotNull().NotEmpty();
            RuleFor(x => x.Senha).NotNull().NotEmpty();
        }
    }
}
