using FluentValidation;

namespace SME.AE.Aplicacao.Comum.Modelos.Usuario
{
    public class SolicitarReiniciarSenhaDtoValidator : AbstractValidator<SolicitarReiniciarSenhaDto>
    {
        public SolicitarReiniciarSenhaDtoValidator()
        {
            RuleFor(v => v.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
