using FluentValidation;
using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosResponsavelResumidoQuery : IRequest<DadosResponsavelAlunoResumido>
    {

        public ObterDadosResponsavelResumidoQuery(string cpfResponsavel)
        {
            CpfResponsavel = cpfResponsavel;
        }

        public string CpfResponsavel { get; set; }
    }

    public class ObterDadosResponsavelResumidoQueryValidator : AbstractValidator<ObterDadosResponsavelResumidoQuery>
    {
        public ObterDadosResponsavelResumidoQueryValidator()
        {
            RuleFor(x => x.CpfResponsavel).NotEmpty().WithMessage("O CPF é Obrigátorio");
        }
    }
}
