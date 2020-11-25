using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IDadosLeituraRepositorio
    {
        Task<IEnumerable<DadosLeituraComunicadosResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe, long notificaoId, int modoVisualizacao);

        Task<IEnumerable<DadosLeituraComunicadosResultado>> ObterDadosLeituraComunicadosPorDre(long notificaoId, int modoVisualizacao);
    }
}


