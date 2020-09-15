using SME.AE.Dominio.Entidades;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface ITermosDeUsoRepositorio : IBaseRepositorio<TermosDeUso>
    {
        Task<TermosDeUso> ObterUltimaVersao();
        Task<TermosDeUso> ObterPorId(long id);
        new Task<long> SalvarAsync(TermosDeUso termo);
    }
}
