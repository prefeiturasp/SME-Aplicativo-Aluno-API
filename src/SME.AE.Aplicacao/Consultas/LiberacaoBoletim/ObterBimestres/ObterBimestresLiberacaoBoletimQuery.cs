using FluentValidation;
using MediatR;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterBimestresLiberacaoBoletimQuery : IRequest<int[]>
    {
        public ObterBimestresLiberacaoBoletimQuery(string turmaCodigo)
        {
            TurmaCodigo = turmaCodigo;
        }

        public string TurmaCodigo { get; set; }

        public class ObterBimestresLiberacaoBoletimQueryValidator : AbstractValidator<ObterBimestresLiberacaoBoletimQuery>
        {
            public ObterBimestresLiberacaoBoletimQueryValidator()
            {
                RuleFor(c => c.TurmaCodigo)
                    .NotEmpty()
                    .WithMessage("O código da turma deve ser informado.");
            }
        }
    }
}
