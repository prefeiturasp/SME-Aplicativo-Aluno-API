using FluentValidation;
using MediatR;
using System.Collections.Generic;

namespace SME.AE.Aplicacao
{
    public class ObterDadosReponsavelPorCpfQuery : IRequest<IEnumerable<ResponsavelAlunoDetalhadoEolDto>>
    {
        public ObterDadosReponsavelPorCpfQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }

    public class ObterDadosResumidosReponsavelPorCpfQueryValidator : AbstractValidator<ObterDadosReponsavelPorCpfQuery>
    {
        public ObterDadosResumidosReponsavelPorCpfQueryValidator()
        {
            RuleFor(v => v.Cpf).NotNull().WithMessage("O campo CPF é obrigatório");
            RuleFor(v => v.Cpf).Length(11).WithMessage("O campo CPF deve ter 11 caracteres");
            RuleFor(v => v.Cpf).ValidarCpf().WithMessage("CPF Inválido");
        }
    }
}
