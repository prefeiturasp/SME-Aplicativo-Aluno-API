using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.Notas
{
    public class ObterNotasAlunoQueryValidator : AbstractValidator<ObterNotasAlunoQuery>
    {
        public ObterNotasAlunoQueryValidator()
        {
            RuleFor(x => x.CodigoUe).NotEmpty().WithMessage("O código da Unidade Escolar (UE) é obrigatório");
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("O código da turma é obrigatório");
            RuleFor(x => x.CodigoAluno).NotEmpty().WithMessage("O código do Aluno é obrigatório");

            RuleFor(x => x.AnoLetivo).NotEmpty().WithMessage("Ano letivo é obrigatório");
            RuleFor(x => x.AnoLetivo).InclusiveBetween(2020, 9999).WithMessage("Ano letivo deve estar entre 2020 e 9999");
        }
    }
}
