using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioRepository : IBaseRepositorio<Usuario>
    {
        Task<Usuario> ObterPorCpf(string cpf);
        Task Criar(Usuario usuario);
        Task AtualizaUltimoLoginUsuario(string cpf);
        Task ExcluirUsuario(string cpf);
        Task CriaUsuarioDispositivo(long usuarioId, string dispositivoId);
        Task<bool> RemoveUsuarioDispositivo(long idUsuario, string idDispositivo);
        Task <bool> ExisteUsuarioDispositivo(long idUsuario, string idDispositivo);
        Task<IEnumerable<string>> ObterTodos();
    }
}
