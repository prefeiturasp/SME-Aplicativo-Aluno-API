using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Aluno
{
    class DadosAlunoUseCaseValidation : AbstractValidator<DadosAlunoCommand>
    {
        public DadosAlunoUseCaseValidation()
        {
            RuleFor(v => v.Cpf).NotNull().WithMessage("O campo CPF é obrigatório");
            RuleFor(v => v.Cpf).Length(11).WithMessage("O campo CPF deve ter 11 caracteres");
            RuleFor(v => v.Cpf).ValidarCpf().WithMessage("CPF Inválido");
        }
    }
}
