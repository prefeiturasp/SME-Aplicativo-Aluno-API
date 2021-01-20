using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosPorDreUeCpfResponsavelQueryValidator : AbstractValidator<ObterDadosAlunosPorDreUeCpfResponsavelQuery>
    {
        public ObterDadosAlunosPorDreUeCpfResponsavelQueryValidator()
        {
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O Código da UE é Obrigátorio");
            RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
