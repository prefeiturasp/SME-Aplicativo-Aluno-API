using FluentValidation;
using MediatR;

namespace SME.AE.Aplicacao
{
    public class VerificaPalavraProibidaPodePersistirCommand : IRequest<bool>
    {

        public VerificaPalavraProibidaPodePersistirCommand(string texto)
        {
            Texto = texto;
        }
        public string Texto { get; set; }
    }

    public class VerificaPalavraProibidaPodePersistirCommandValidator : AbstractValidator<VerificaPalavraProibidaPodePersistirCommand>
    {
        public VerificaPalavraProibidaPodePersistirCommandValidator()
        {
            RuleFor(x => x.Texto)
                   .NotEmpty()
                   .WithMessage("O texto deve ser informado!");
        }
    }
}
