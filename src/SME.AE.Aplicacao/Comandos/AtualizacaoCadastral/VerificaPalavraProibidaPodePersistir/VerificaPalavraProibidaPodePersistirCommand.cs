using FluentValidation;
using MediatR;

namespace SME.AE.Aplicacao
{
    public class VerificaPalavraProibidaPodePersistirCommand : IRequest<bool>
    {

        public VerificaPalavraProibidaPodePersistirCommand(string nome)
        {
            Nome = nome;
        }
        public string Nome { get; set; }
    }

    public class VerificaPalavraProibidaPodePersistirCommandValidator : AbstractValidator<VerificaPalavraProibidaPodePersistirCommand>
    {
        public VerificaPalavraProibidaPodePersistirCommandValidator()
        {
            RuleFor(x => x.Nome)
                   .NotEmpty()
                   .WithMessage("O nome deve ser informado!");
        }
    }
}
