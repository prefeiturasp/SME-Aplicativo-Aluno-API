using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioRepository
    {
        Task<Usuario> ObterPorCpf(string cpf);
        Task Criar(Usuario usuario);
        Task AtualizaUltimoLoginUsuario(string cpf);
        Task ExcluirUsuario(string cpf);
        Task CriaUsuarioDispositivo(int usuarioId, string dispositivoId);
        Task RemoveUsuarioDispositivo(long idUsuario, string idDispositivo);
    }
}
