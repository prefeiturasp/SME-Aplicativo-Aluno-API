using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SME.AE.Dominio.Entidades;
using SME.AE.Aplicacao.Comum.Modelos;
using AutoMapper;
using SME.AE.Aplicacao.Comum.Enumeradores;

namespace SME.AE.Aplicacao
{
    public class CriarNotificacaoUseCase : ICriarNotificacaoUseCase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;

        public CriarNotificacaoUseCase(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<NotificacaoSgpDto> Executar(NotificacaoSgpDto notificacao)
        {
             await mediator.Send(new CriarNotificacaoCommand(mapper.Map<Notificacao>(notificacao)));

            return mapper.Map<NotificacaoSgpDto>(notificacao);
        }

        private async Task EnviarNotificacaoImediataAsync(NotificacaoSgpDto notificacao)
        {
            var dataEnvio = TimeZoneInfo.ConvertTimeToUtc(notificacao.DataEnvio);
            var agora = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

            if (dataEnvio > agora)
                return;

            List<int> grupos = notificacao.Grupo.Split(',').Select(i => Int32.Parse(i)).ToList();

            await mediator.Send(new EnviarNotificacaoPorGrupoCommand(mapper.Map<Notificacao>(notificacao), grupos));
        }
    }
}
