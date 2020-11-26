using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterDadosDeLeituraComunicadosUseCase : IObterDadosDeLeituraComunicadosUseCase
    {

        private readonly IMediator mediator;

        public ObterDadosDeLeituraComunicadosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<DadosLeituraComunicadosResultado>> Executar(string codigoDre, string codigoUe, long notificacaoId, int modoVisualizacao)
        {
            return mediator.Send(new ObterDadosLeituraComunicadosQuery(codigoDre, codigoUe, notificacaoId, modoVisualizacao));
        }
    }

    public class ObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase : IObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase
    {

        private readonly IMediator mediator;

        public ObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<DadosLeituraComunicadosResultado>> Executar(long notificacaoId, int modoVisualizacao)
        {
            throw new System.NotImplementedException();
        }
    }
}
