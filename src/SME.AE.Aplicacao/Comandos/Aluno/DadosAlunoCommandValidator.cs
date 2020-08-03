using FluentValidation;

namespace SME.AE.Aplicacao.Comandos.Aluno
{
    class DadosAlunoCommandValidator : AbstractValidator<DadosAlunoCommand>
    {
        public DadosAlunoCommandValidator()
        {
            RuleFor(v => v.Cpf).NotNull().WithMessage("O campo CPF é obrigatório");
            RuleFor(v => v.Cpf).ValidarCpf().WithMessage("CPF Inválido").When(x => !string.IsNullOrWhiteSpace(x.Cpf));
        }
    }
}
