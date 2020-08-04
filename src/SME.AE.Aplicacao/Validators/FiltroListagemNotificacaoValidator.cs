using FluentValidation;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Validators
{
    public class FiltroListagemNotificacaoValidator : AbstractValidator<FiltroListagemNotificacaoDto>
    {
        public FiltroListagemNotificacaoValidator()
        {
            RuleFor(x => x.CodigoAluno).GreaterThan(0).WithMessage("O codigo do Aluno é Obrigátorio");
        }
    }
}
