using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase : IObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase
    {

        private readonly IMediator mediator;

        public ObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Executar(long notificacaoId, ModoVisualizacao modoVisualizacao)
        {
            throw new System.NotImplementedException();
        }
    }
}
