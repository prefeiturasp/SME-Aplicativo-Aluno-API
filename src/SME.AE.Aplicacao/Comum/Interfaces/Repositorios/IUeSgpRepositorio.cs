using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUeSgpRepositorio
    {
        Task<long[]> ObterIdUes();
    }
}
