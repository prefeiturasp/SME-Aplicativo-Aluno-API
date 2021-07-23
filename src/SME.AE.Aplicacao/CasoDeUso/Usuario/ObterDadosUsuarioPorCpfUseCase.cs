using MediatR;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class ObterDadosUsuarioPorCpfUseCase : IObterDadosUsuarioPorCpfUseCase
    {
        private readonly IMediator mediator;

        public ObterDadosUsuarioPorCpfUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }
        public async Task<UsuarioDadosDetalhesDto> Executar(string cpf)
        {
            return await mediator.Send(new ObterUsuarioDetalhesPorCpfQuery(cpf));
        }
    }
}
