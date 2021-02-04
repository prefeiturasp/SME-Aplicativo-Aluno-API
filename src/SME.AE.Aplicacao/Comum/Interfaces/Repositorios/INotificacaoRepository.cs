using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.NotificacaoPorUsuario;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface INotificacaoRepository : IBaseRepositorio<Notificacao>
    {
        public Task<IEnumerable<NotificacaoPorUsuario>> ObterPorGrupoUsuario(string grupo, string cpf);

        public Task Criar(Notificacao notificacao);

        public Task Atualizar(AtualizarNotificacaoDto notificacao);

        public Task<bool> Remover(Notificacao notificacao);

        public Task<IDictionary<string, object>> ObterGruposDoResponsavel(string cpf, string grupos, string nomeGrupos);

        public Task<IEnumerable<string>> ObterResponsaveisPorGrupo(string where);

        Task<IEnumerable<NotificacaoTurma>> ObterTurmasPorNotificacao(long id);

        public Task InserirNotificacaoAluno(NotificacaoAluno notificacaoAluno);

        public Task InserirNotificacaoTurma(NotificacaoTurma notificacaoTurma);
        Task<IEnumerable<NotificacaoResposta>> ListarNotificacoes(string gruposId, string codigoUe, string codigoDre, string codigoTurma, string codigoAluno, long usuarioId, string serieResumida);
        Task<NotificacaoResposta> NotificacaoPorId(long Id);
        Task<IEnumerable<NotificacaoSgpDto>> ListarNotificacoesNaoEnviadas();
        Task<IEnumerable<NotificacaoAlunoResposta>> ObterNotificacoesAlunoPorId(long id);
    }
}