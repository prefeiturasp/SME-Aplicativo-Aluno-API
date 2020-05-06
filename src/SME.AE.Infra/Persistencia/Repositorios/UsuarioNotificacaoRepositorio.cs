using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class UsuarioNotificacaoRepositorio : IUsuarioNotificacaoRepositorio
    {
        public Task<UsuarioNotificacao> Atualizar(Notificacao notificacao)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioNotificacao> Criar(Notificacao notificacao)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioNotificacao> ObterPorId(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Remover(Notificacao notificacao)
        {
            throw new NotImplementedException();
        }
    }
}
