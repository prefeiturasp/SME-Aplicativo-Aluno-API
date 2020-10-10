using AutoMapper;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Comum.Excecoes;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao
{
    public class AtualizarNotificacaoUseCase : IAtualizarNotificacaoUseCase
    {
        private readonly IMediator mediator;

        public AtualizarNotificacaoUseCase(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public async Task<AtualizacaoNotificacaoResposta> Executar(NotificacaoSgpDto notificacao)
        {
            var resultado = await mediator.Send(new AtualizarNotificacaoCommand(notificacao.Id, notificacao.Titulo, notificacao.Mensagem, notificacao.DataExpiracao, notificacao.AlteradoPor));
            if (resultado == null)
                throw new NegocioException("Não foi possível atualizar o comunicado na base do Escola Aqui!");

            return resultado;
        }
    }
}
