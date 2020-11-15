using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IDadosLeituraRepositorio
    {
        Task<IEnumerable<DadosLeituraResultado>> ObterDadosLeituraComunicados(string codigoDre, string codigoUe);

        Task<IEnumerable<DadosLeituraResultado>> ObterDadosLeituraComunicadosPorDre();
    }
}


