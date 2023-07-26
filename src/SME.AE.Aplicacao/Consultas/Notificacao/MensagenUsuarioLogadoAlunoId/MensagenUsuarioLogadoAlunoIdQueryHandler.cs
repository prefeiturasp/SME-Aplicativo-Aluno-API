using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagenUsuarioLogadoAlunoIdQueryHandler : IRequestHandler<MensagenUsuarioLogadoAlunoIdQuery, NotificacaoResposta>
    {
        private readonly INotificacaoRepositorio notificacaoRepository;
        
        public MensagenUsuarioLogadoAlunoIdQueryHandler(INotificacaoRepositorio notificacaoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
        }

        public async Task<NotificacaoResposta> Handle(MensagenUsuarioLogadoAlunoIdQuery request, CancellationToken cancellationToken)
        {
            return await notificacaoRepository.NotificacaoPorId(request.Id);
        }
    }
}
