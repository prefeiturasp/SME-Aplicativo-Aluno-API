using FluentValidation;
using MediatR;

namespace SME.AE.Aplicacao
{
    public class ObterFrequenciaGlobalAlunoQuery : IRequest<double?>
    {
        public string TurmaCodigo { get; set; }
        public string AlunoCodigo { get; set; }
    }
    public class ObterFrequenciaGlobalAlunoQueryValidator : AbstractValidator<ObterFrequenciaGlobalAlunoQuery>
    {
        public ObterFrequenciaGlobalAlunoQueryValidator()
        {
            RuleFor(c => c.TurmaCodigo)
                .NotEmpty()
                .WithMessage("O código da turma deve ser informado.");

            RuleFor(c => c.AlunoCodigo)
                .NotEmpty()
                .WithMessage("O código do aluno deve ser informado.");
        }
    }
}
