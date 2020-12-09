using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IDadosLeituraRepositorio
    {
        Task<IEnumerable<DadosConsolidacaoNotificacaoResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe, long notificacaoId);

        Task<IEnumerable<DadosConsolidacaoNotificacaoResultado>> ObterDadosLeituraComunicadosPorDre(long notificacaoId);
        Task<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>> ObterDadosLeituraModalidade(string codigoDre, string codigoUe, long notificacaoId, bool porResponsavel);
    }
}


