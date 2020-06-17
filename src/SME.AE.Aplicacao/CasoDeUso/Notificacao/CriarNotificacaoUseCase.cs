using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MediatR;
using Sentry;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo;
using SME.AE.Aplicacao.Comandos.Token.Criar;

namespace SME.AE.Aplicacao.CasoDeUso.Notificacao
{
    public class CriarNotificacaoUseCase
    {
        public static async Task<Dominio.Entidades.Notificacao> Executar(
            IMediator mediator, Dominio.Entidades.Notificacao notificacao)
        {
            Dominio.Entidades.Notificacao resultado;

            try
            {
                resultado = await mediator.Send(new CriarNotificacaoCommand(notificacao));
                await EnviarNotificacaoImediataAsync(mediator, notificacao);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                throw ex;
            }

            return resultado;
        }

        private static async Task EnviarNotificacaoImediataAsync(
            IMediator mediator, Dominio.Entidades.Notificacao notificacao)
        {
            try
            {
                var dataEnvio = TimeZoneInfo.ConvertTimeToUtc(notificacao.DataEnvio);
                var agora = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

                if (dataEnvio <= agora)
                {
                    List<int> grupos = notificacao.Grupo.Split(',').Select(i => Int32.Parse(i)).ToList();
                    await mediator.Send(new EnviarNotificacaoPorGrupoCommand(notificacao, grupos));
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
