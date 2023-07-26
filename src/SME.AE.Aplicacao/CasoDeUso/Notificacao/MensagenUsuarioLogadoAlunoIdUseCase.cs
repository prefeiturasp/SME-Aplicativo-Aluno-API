using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.Notificacao.ListarNotificacaoAluno;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class MensagenUsuarioLogadoAlunoIdUseCase : IMensagenUsuarioLogadoAlunoIdUseCase
    {
        private readonly IMediator mediator;

        public MensagenUsuarioLogadoAlunoIdUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<NotificacaoResposta> Executar(long id)
        {
            var notificacao = await mediator.Send(new MensagenUsuarioLogadoAlunoIdQuery { Id = id });
            return notificacao;
        }
    }
}
