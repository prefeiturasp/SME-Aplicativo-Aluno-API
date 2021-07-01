using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterModalidadeDeEnsinoQueryValidator : AbstractValidator<ObterModalidadeDeEnsinoQuery>
    {
        public ObterModalidadeDeEnsinoQueryValidator()
        {
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("O código da turma deve ser informado");
        }
    }
}
