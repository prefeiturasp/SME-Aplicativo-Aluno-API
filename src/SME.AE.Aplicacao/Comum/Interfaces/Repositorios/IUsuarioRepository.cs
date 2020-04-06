using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorCpf(string cpf);
    }
}
