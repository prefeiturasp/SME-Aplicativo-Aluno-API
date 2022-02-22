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
        private readonly IGrupoComunicadoRepository grupoComunicadoRepository;

        public MensagenUsuarioLogadoAlunoIdQueryHandler(INotificacaoRepositorio notificacaoRepository, IGrupoComunicadoRepository grupoComunicadoRepository)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
            this.grupoComunicadoRepository = grupoComunicadoRepository ?? throw new ArgumentNullException(nameof(grupoComunicadoRepository));
        }

        public async Task<NotificacaoResposta> Handle(MensagenUsuarioLogadoAlunoIdQuery request, CancellationToken cancellationToken)
        {
            return await notificacaoRepository.NotificacaoPorId(request.Id);
        }
    }
}
