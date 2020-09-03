using MediatR;
using Org.BouncyCastle.Crypto;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno
{
    public class MensagenUsuarioLogadoAlunoIdQueryHandler : IRequestHandler<MensagenUsuarioLogadoAlunoIdQuery, NotificacaoResposta>
    {
        private readonly INotificacaoRepository notificacaoRepository;
        private readonly IGrupoComunicadoRepository grupoComunicadoRepository;

        public MensagenUsuarioLogadoAlunoIdQueryHandler(INotificacaoRepository notificacaoRepository, IGrupoComunicadoRepository grupoComunicadoRepository)
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
