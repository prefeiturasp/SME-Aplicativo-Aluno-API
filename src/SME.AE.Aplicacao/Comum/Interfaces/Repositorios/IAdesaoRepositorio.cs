using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IAdesaoRepositorio : IBaseRepositorio<Adesao>
    {
        Task<IEnumerable<TotaisAdesaoResultado>> ObterTotaisAdesao(string codigoDre, string codigoUe);
    }
}
