using FluentValidation;
using MediatR;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterDadosResumidosReponsavelPorCpfQuery : IRequest<ResponsavelAlunoEolResumidoDto>
    {
        public ObterDadosResumidosReponsavelPorCpfQuery(string cpf)
        {
            Cpf = cpf;
        }

        public string Cpf { get; set; }
    }

    public class ObterDadosResumidosReponsavelPorCpfQueryValidator : AbstractValidator<ObterDadosResumidosReponsavelPorCpfQuery>
    {
        public ObterDadosResumidosReponsavelPorCpfQueryValidator()
        {
            RuleFor(v => v.Cpf).NotNull().WithMessage("O campo CPF é obrigatório");
            RuleFor(v => v.Cpf).Length(11).WithMessage("O campo CPF deve ter 11 caracteres");
            RuleFor(v => v.Cpf).ValidarCpf().WithMessage("CPF Inválido");
        }
    }
}
