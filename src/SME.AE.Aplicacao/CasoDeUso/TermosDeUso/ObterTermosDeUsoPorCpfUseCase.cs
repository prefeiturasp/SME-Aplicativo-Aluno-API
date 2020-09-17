using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.TermosDeUso
{
    public class ObterTermosDeUsoPorCpfUseCase : IObterTermosDeUsoPorCpfUseCase
    {
        private readonly IMediator mediator;

        public ObterTermosDeUsoPorCpfUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<RetornoTermosDeUsoDto> Executar(string cpf)
        {
            return await mediator.Send(new ObterTermosDeUsoPorCpfQuery(cpf));
        }
    }
}
