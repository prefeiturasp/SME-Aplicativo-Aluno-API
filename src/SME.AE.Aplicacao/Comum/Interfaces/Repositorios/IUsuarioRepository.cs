using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioRepository : IBaseRepositorio<Usuario>
    {
        Task<Usuario> ObterUsuarioNaoExcluidoPorCpf(string cpf);
        Task<Usuario> ObterPorCpf(string cpf);
        Task AtualizarPrimeiroAcesso(long id, bool primeiroAcesso);
        Task AtualizarEmailTelefone(long id, string email, string celular);
        Task AtualizaUltimoLoginUsuario(string cpf);
        Task ExcluirUsuario(string cpf);
        Task<Usuario> ObterUsuarioPorTokenAutenticacao(string token);
        Task AltualizarUltimoAcessoPrimeiroUsuario(Usuario usuario);
        Task CriaUsuarioDispositivo(long usuarioId, string dispositivoId);
        Task<bool> RemoveUsuarioDispositivo(long idUsuario, string idDispositivo);
        Task<bool> ExisteUsuarioDispositivo(long idUsuario, string idDispositivo);
        Task<IEnumerable<string>> ObterTodos();
        new Task<Usuario> ObterPorIdAsync(long id);
        new Task RemoverAsync(long id);
        new Task RemoverAsync(Usuario usuario);
        new Task<long> SalvarAsync(Usuario usuario);
        Task<long> ObterTotalUsuariosComAcessoIncompleto(List<string> cpfs);
        Task<long> ObterTotalUsuariosValidos(List<string> cpfs);
        Task<IEnumerable<Usuario>> ObterTodosUsuariosAtivos();        
    }
}
