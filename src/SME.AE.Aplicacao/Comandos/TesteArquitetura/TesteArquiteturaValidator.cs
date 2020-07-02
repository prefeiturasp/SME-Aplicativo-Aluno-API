using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.TesteArquitetura
{
    public class TesteArquiteturaValidator : AbstractValidator<TesteArquiteturaCommand>
    {
        public TesteArquiteturaValidator()
        {
            RuleFor(x => x.Cpf).NotNull().NotEmpty().ValidarCpf().WithMessage("O CPF é obrigátorio");
            RuleFor(x => x.Id).NotNull().NotEmpty().WithMessage("O Id é obrigátorio");
            RuleFor(x => x.Usuario).NotNull().SetValidator(new UsuarioValidator()).WithMessage("O Usuário é obrigátorio");
        }
    }
}
