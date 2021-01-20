using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterDadosDeLeituraModalidadeUseCase : IObterDadosDeLeituraModalidadeUseCase
    {

        private readonly IMediator mediator;

        public ObterDadosDeLeituraModalidadeUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> Executar(string codigoDre, string codigoUe, long notificacaoId, ModoVisualizacao modoVisualizacao)
        {
            return mediator.Send(new ObterDadosLeituraModalidadeQuery(codigoDre, codigoUe, notificacaoId, modoVisualizacao));
        }
    }
}
