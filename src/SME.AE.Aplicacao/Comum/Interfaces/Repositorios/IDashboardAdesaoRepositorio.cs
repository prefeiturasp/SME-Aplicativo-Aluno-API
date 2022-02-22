using SME.AE.Aplicacao.Comum.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IDashboardAdesaoRepositorio
    {
        Task IncluiOuAtualizaPorDreUeTurmaEmBatch(IEnumerable<DashboardAdesaoDto> listaAdesao);
    }
}
