using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotificacaoRepository
    {
        public Task<IEnumerable<NotificacaoPorUsuario>> ObterPorGrupoUsuario(string grupo, string cpf);

        public Task<Notificacao> ObterPorId(long id);
        
        public Task<Notificacao> Criar(Notificacao notificacao);
        
        public Task<Notificacao> Atualizar(Notificacao notificacao);
        
        public Task<bool> Remover(Notificacao notificacao);

        public Task<IDictionary<string, object>> ObterGruposDoResponsavel(string cpf, string grupos,string nomeGrupos);
        public Task<IEnumerable<string>> ObterResponsaveisPorGrupo(string where);
    }
}