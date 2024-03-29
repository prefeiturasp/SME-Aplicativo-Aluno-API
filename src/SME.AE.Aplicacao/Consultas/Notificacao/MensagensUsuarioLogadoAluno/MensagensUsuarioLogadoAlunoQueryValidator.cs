﻿using FluentValidation;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagensUsuarioLogadoAlunoQueryValidator : AbstractValidator<ParametrosMensagensUsuarioLogado>
    {
        public MensagensUsuarioLogadoAlunoQueryValidator()
        {
            RuleFor(x => x.CodigoAluno).NotEmpty().WithMessage("É necessário informar o codigo do Aluno");
            RuleFor(x => x.CodigoDRE).NotEmpty().WithMessage("É necessário informar o codigo da DRE");
            RuleFor(x => x.CodigoTurma).NotEmpty().WithMessage("É necessário informar o codigo da Turma");
            RuleFor(x => x.CodigoUE).NotEmpty().WithMessage("É necessário informar o codigo da UE");
            RuleFor(x => x.ModalidadesId).NotEmpty().WithMessage("É necessário informar as modalidades do Aluno");
            RuleFor(x => x.DataUltimaConsulta).NotEmpty().WithMessage("É necessário informar a data da última consulta");
        }
    }
}