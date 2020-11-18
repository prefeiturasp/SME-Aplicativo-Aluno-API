using SME.AE.Dominio.Entidades;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface ITermosDeUsoRepositorio : IBaseRepositorio<TermosDeUso>
    {
        Task<TermosDeUso> ObterTermosDeUsoPorCpf(string cpf);

        Task<TermosDeUso> ObterUltimaVersaoTermosDeUso();
        Task<TermosDeUso> ObterPorId(long id);
        new Task<long> SalvarAsync(TermosDeUso termo);
    }
}
