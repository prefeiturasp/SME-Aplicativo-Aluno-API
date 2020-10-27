using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IAdesaoRepositorio
    {
        Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoSme();

        Task<IEnumerable<TotaisAdesaoResultado>> ObterDadosAdesaoAgrupadosPorDreUeETurma(string codigoDre, string codigoUe);
    }
}


