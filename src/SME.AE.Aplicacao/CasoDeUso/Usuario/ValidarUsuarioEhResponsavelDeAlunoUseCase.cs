using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Consultas;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ValidarUsuarioEhResponsavelDeAlunoUseCase : IValidarUsuarioEhResponsavelDeAlunoUseCase
    {
        private readonly IMediator mediator;

        public ValidarUsuarioEhResponsavelDeAlunoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Executar(string cpf)
        {
            return await mediator.Send(new ValidarUsuarioEhResponsavelDeAlunoQuery(cpf));
        }
    }
}
