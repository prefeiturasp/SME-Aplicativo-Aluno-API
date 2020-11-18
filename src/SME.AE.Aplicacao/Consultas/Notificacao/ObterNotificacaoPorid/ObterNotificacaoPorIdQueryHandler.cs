using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Utilitarios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ObterNotificacaoPorid
{
    public class ObterNotificacaoPorIdQueryHandler : IRequestHandler<ObterNotificacaoPorIdQuery, NotificacaoResposta>
    {
        private readonly INotificacaoRepository _repository;

        public ObterNotificacaoPorIdQueryHandler(INotificacaoRepository repository, IGrupoComunicadoRepository grupoComunicadoRepository)
        {
            this._repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<NotificacaoResposta> Handle(ObterNotificacaoPorIdQuery request, CancellationToken cancellationToken)
        {
            var notificacao = await _repository.ObterPorIdAsync(request.Id);

            var turmas = await _repository.ObterTurmasPorNotificacao(notificacao.Id);

            var notificacaoResposta = new NotificacaoResposta()
            {
                AlteradoEm = notificacao.AlteradoEm,
                AlteradoPor = notificacao.AlteradoPor,
                CriadoEm = notificacao.CriadoEm,
                CriadoPor = notificacao.CriadoPor,
                Id = notificacao.Id,
                CodigoDre = notificacao.CodigoDre,
                CodigoUe = notificacao.CodigoUe,
                Turmas = turmas == default ? default : turmas,
                DataEnvio = notificacao.DataEnvio,
                DataExpiracao = notificacao.DataExpiracao,
                Mensagem = notificacao.Mensagem,
                Titulo = notificacao.Titulo,
                TipoComunicado = notificacao.TipoComunicado,
                CategoriaNotificacao = notificacao.CategoriaNotificacao,
                GruposId = notificacao.Grupo.ToStringEnumerable().ToArray(),
                SeriesResumidas = notificacao.SeriesResumidas
            };

            return notificacaoResposta;
        }

        private IEnumerable<Grupo> SelecionarGrupos(string grupo, IEnumerable<GrupoComunicado> grupos)
        {
            var ids = grupo.Split(',').Select(x => Convert.ToInt64(x));
            return grupos.Where(w => ids.Contains(w.Id)).Select(s => new Grupo { Codigo = s.Id, Nome = s.Nome });
        }
    }
}
