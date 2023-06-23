using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IComponenteCurricularSgpRepositorio
    {
        Task<string> ObterDescricaoComponenteCurricular(long codigoComponenteCurricular);
    }
}
