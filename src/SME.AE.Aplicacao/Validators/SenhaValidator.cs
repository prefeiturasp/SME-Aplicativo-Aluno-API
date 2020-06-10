using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class SenhaValidator : AbstractValidator<NovaSenhaDto>
    {
        public SenhaValidator()
        {            
            RuleFor(x => x.NovaSenha).MinimumLength(8);
            RuleFor(x => x.NovaSenha).MaximumLength(12);
            RuleFor(x => x.NovaSenha).Must(x => x.Contains(" "));
            RuleFor(x => x.NovaSenha).Matches(@"(?=.*?[A-Z])(?=.*?[a-z])(?=((?=.*[!@#$\-%&/\\\[\]|*()_=+])|(?=.*?[0-9]+)))");
        }
    }
}
