using FluentValidation;
using SME.AE.Aplicacao.Validators;

namespace SME.AE.Aplicacao.Comandos.CoreSSO.Usuario
{
    public class CriarUsuarioCoreSSOCommandValidator : AbstractValidator<CriarUsuarioCoreSSOCommand>
    {
        public CriarUsuarioCoreSSOCommandValidator()
        {
            RuleFor(x => x.Usuario).NotNull().SetValidator(new UsuarioCoreSSOValidator()).WithMessage("O Usuário é Obrigátorio");            
        }
    }
}
