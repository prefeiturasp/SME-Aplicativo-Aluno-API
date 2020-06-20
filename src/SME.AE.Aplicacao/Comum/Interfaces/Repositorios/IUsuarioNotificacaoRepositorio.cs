using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioNotificacaoRepositorio
    {
        public Task<UsuarioNotificacao> ObterPorId(long id);
        
        public Task<bool> Criar(UsuarioNotificacao notificacao);
        
        public  Task<bool> Remover(long id);
        public Task<bool> RemoverPorId(long id);
        Task<UsuarioNotificacao> Selecionar(UsuarioNotificacao usuarioNotificacao);
        public Task<UsuarioNotificacao> ObterPorNotificacaoIdEhUsuarioCpf(long notificacaoId, string usuarioCpf, long dreCodigoEol, string ueCodigoEol);
        public Task<bool> Atualizar(UsuarioNotificacao notificacao);
    }
}