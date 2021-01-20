using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard
{
    public interface IObterDadosDeLeituraComunicadosAgrupadosPorDreUseCase
    {
        Task<IEnumerable<DadosLeituraComunicadosResultado>> Executar(long notificacaoId, ModoVisualizacao modoVisualizacao);
    }
}
