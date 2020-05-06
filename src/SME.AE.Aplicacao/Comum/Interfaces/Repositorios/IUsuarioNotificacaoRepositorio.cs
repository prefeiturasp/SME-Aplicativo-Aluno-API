using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IUsuarioNotificacaoRepositorio
    {
        public Task<UsuarioNotificacao> ObterPorId(long id);
        
        public Task<UsuarioNotificacao> Criar(Notificacao notificacao);
        
        public Task<UsuarioNotificacao> Atualizar(Notificacao notificacao);

        public  Task<bool> Remover(long notificacaoId);

    }
}