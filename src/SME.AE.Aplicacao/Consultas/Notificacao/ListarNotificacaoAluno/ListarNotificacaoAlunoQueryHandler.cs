using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class ListarNotificacaoAlunoQueryHandler : IRequestHandler<ListarNotificacaoAlunoQuery, IEnumerable<NotificacaoResposta>>
    {
        private readonly INotificacaoRepository notificacaoRepository;

        public ListarNotificacaoAlunoQueryHandler(INotificacaoRepository notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<IEnumerable<NotificacaoResposta>> Handle(ListarNotificacaoAlunoQuery request, CancellationToken cancellationToken)
        {
            return await notificacaoRepository.ListarNotificacoes(request.GruposId, request.CodigoUE, request.CodigoDRE, request.CodigoTurma, request.CodigoAluno, request.CodigoUsuario);
        }
    }
}
