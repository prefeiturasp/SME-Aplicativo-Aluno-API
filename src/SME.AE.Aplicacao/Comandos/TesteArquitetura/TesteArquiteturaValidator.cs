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
            RuleFor(x => x.Cpf).NotNull().NotEmpty().ValidarCpf();
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.Usuario).NotNull().SetValidator(new UsuarioValidator());
        }
    }
}
