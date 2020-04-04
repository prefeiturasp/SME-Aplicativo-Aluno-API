using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario
{
    class CriarUsuarioUseCaseValidatior : AbstractValidator<CriarUsuarioCommand>
    {
        public CriarUsuarioUseCaseValidatior()
        {
            RuleFor(v => v.Cpf).NotNull().WithMessage("O campo CPF é obrigatório");
            RuleFor(v => v.Cpf).Length(11).WithMessage("O campo CPF deve ter 11 caracteres");
            RuleFor(v => v.Cpf).ValidarCpf().WithMessage("CPF Inválido");

            RuleFor(v => v.DataNascimento).NotNull().WithMessage("O campo Data de Nascimento é obrigatório");
            RuleFor(v => v.DataNascimento).DataNascimentoEhValida().WithMessage("Data de nascimento inválida");
        }
    }
}
