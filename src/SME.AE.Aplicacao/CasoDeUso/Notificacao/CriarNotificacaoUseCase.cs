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
using FirebaseAdmin.Messaging;

namespace SME.AE.Aplicacao
{
    public class CriarNotificacaoUseCase : ICriarNotificacaoUseCase
    {
        private readonly IMediator mediator;
        private readonly IMapper mapper;
        public List<Dictionary<String, String>> listaDicionario = new List<Dictionary<String, String>>();


        public CriarNotificacaoUseCase(IMediator mediator, IMapper mapper)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<NotificacaoSgpDto> Executar(NotificacaoSgpDto notificacao)
        {
            await mediator.Send(new CriarNotificacaoCommand(mapper.Map<Notificacao>(notificacao)));

            await EnviarNotificacaoImediataAsync(notificacao);
            return mapper.Map<NotificacaoSgpDto>(notificacao);
        }

        private async Task EnviarNotificacaoImediataAsync(NotificacaoSgpDto notificacao)
        {
            var dataEnvio = TimeZoneInfo.ConvertTimeToUtc(notificacao.DataEnvio);
            var agora = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);

            if (dataEnvio > agora)
                return;

            List<int> grupos = notificacao.Grupo.Split(',').Select(i => Int32.Parse(i)).ToList();//
            string topico = string.Empty;
            Dictionary<string, string> dicionarioNotificacao = new Dictionary<String, String>
            {
                ["Titulo"] = notificacao.Titulo,
                ["Mensagem"] = notificacao.Mensagem,
                ["Id"] = notificacao.Id.ToString(),
                ["CriadoEm"] = notificacao.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                ["click_action"] = "FLUTTER_NOTIFICATION_CLICK",
            };


            var Notificacao = new Notification
            {
                Title = notificacao.Titulo,
                Body = "Você recebeu uma nova mensagem da SME. Clique aqui para visualizar os detalhes.",
            };

            if (notificacao.TipoComunicado == TipoComunicado.SME)
            {
                foreach (var grupo in grupos)
                {
                    var data = new Dictionary<String, String>();
                    data = dicionarioNotificacao;
                    topico = "Grupo-" + grupo.ToString();
                    data.Add("CodigoUe", "UE-" + notificacao.CodigoUe);

                    await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));

                }
            }

            else if (notificacao.TipoComunicado == TipoComunicado.DRE)
            {
                var data = new Dictionary<String, String>();
                data = dicionarioNotificacao;
                topico = "DRE-" + notificacao.CodigoDre;
                data.Add("CodigoDre", "DRE-" + topico);

                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }

            else if (notificacao.TipoComunicado == TipoComunicado.UE)
            {
                var data = new Dictionary<String, String>();
                data = dicionarioNotificacao;
                topico = "DRE-" + notificacao.CodigoUe;
                data.Add("CodigoDre", "DRE-" + topico);

                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }

            else if (notificacao.TipoComunicado == TipoComunicado.UEMOD)
            {
                foreach (var grupo in grupos)
                {
                    var data = new Dictionary<String, String>();
                    data = dicionarioNotificacao;

                    topico = "UE-" + notificacao.CodigoUe + "-MOD-" + grupo;
                    data.Add("CodigoUe", "UE-" + notificacao.CodigoUe);
                    await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
                }
            }

            else if (notificacao.TipoComunicado == TipoComunicado.TURMA)
            {
                foreach (var turma in notificacao.Turma)
                {
                    var data = new Dictionary<String, String>();
                    data = dicionarioNotificacao;

                    topico = "UE-" + notificacao.CodigoUe;
                    data.Add("CodigoUe", "UE-" + notificacao.CodigoUe);
                    await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
                }
            }

            else if (notificacao.TipoComunicado == TipoComunicado.ALUNO)
            {
                foreach (var aluno in notificacao.Alunos)
                {
                    var data = new Dictionary<String, String>();
                    data = dicionarioNotificacao;

                    topico = "ALU-" + aluno;
                    data.Add("CodigoAluno", topico);
                    await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
                }
            }
        }

        private static Message MontaMensagem(string topico, Notification notificacao, Dictionary<string, string> data)
        {
            var Mensagem = new Message();
            Mensagem.Notification = notificacao;
            Mensagem.Data = data;
            Mensagem.Topic = topico;
            return Mensagem;
        }
    }
}