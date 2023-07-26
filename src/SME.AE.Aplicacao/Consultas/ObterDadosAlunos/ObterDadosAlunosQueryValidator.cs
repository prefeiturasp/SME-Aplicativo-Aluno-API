using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQueryValidator : AbstractValidator<ObterDadosAlunosQuery>
    {
        public ObterDadosAlunosQueryValidator()
        {
            //RuleFor(x => x.CpfResponsavel).NotEmpty().WithMessage("O CPF é Obrigátorio");
            //RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O Código da UE é Obrigátorio");
            //RuleFor(x => x.Cpf).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
