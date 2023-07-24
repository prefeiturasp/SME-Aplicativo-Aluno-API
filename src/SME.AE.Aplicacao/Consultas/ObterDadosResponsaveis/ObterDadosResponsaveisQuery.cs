using FluentValidation;
using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosResponsaveisQuery : IRequest<IEnumerable<RetornoUsuarioEol>>
    {

        public ObterDadosResponsaveisQuery(string cpfResponsavel)
        {
            CpfResponsavel = cpfResponsavel;
        }

        public string CpfResponsavel { get; set; }
    }

    public class ObterDadosResponsaveisQueryValidator : AbstractValidator<ObterDadosResponsaveisQuery>
    {
        public ObterDadosResponsaveisQueryValidator()
        {
            RuleFor(x => x.CpfResponsavel).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
