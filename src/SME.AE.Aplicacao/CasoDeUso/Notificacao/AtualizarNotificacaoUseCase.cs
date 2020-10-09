using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Aplicacao
{
    public class AtualizarNotificacaoUseCase : IAtualizarNotificacaoUseCase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public AtualizarNotificacaoUseCase(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<NotificacaoSgpDto> Executar(NotificacaoSgpDto notificacao)
        {
            var resultado = await mediator.Send(new AtualizarNotificacaoCommand(mapper.Map<Notificacao>(notificacao)));
            
            if (resultado == null)
                throw new NegocioException("Não foi possível atualizar o comunicado na base do Escola Aqui!");

            return mapper.Map<NotificacaoSgpDto>(resultado);
        }
    }
}