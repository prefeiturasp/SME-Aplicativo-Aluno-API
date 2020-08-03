using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class ListarNotificacaoAlunoQueryValidator : AbstractValidator<ListarNotificacaoAlunoQuery>
    {
        public ListarNotificacaoAlunoQueryValidator()
        {
            RuleFor(x => x.CodigoAluno).NotEmpty().WithMessage("É necessário informar o codigo do Aluno");

            RuleFor(x => x.CodigoDRE).NotEmpty().WithMessage("É necessário informar o codigo da DRE");

            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("É necessário informar o codigo da Turma");

            RuleFor(x => x.CodigoUE).NotEmpty().WithMessage("É necessário informar o codigo da UE");

            RuleFor(x => x.GruposId).NotEmpty().WithMessage("É necessário informar os grupos do Aluno");
        }
    }
}