using AutoMapper;
using FirebaseAdmin.Messaging;
using MediatR;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum.Utilitarios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

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
            notificacao.DataEnvio = notificacao.DataEnvio.Date;
            var dataEnvio = TimeZoneInfo.ConvertTimeToUtc(notificacao.DataEnvio).Date;
            var hoje = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now).Date;

            notificacao.EnviadoPushNotification = (hoje >= dataEnvio);

            await mediator.Send(new CriarNotificacaoCommand(mapper.Map<Notificacao>(notificacao)));

            if (notificacao.EnviadoPushNotification)
            {
                notificacao.InserirCategoria();
                await EnviarNotificacaoImediataAsync(notificacao);
            }

            return notificacao;
        }

        public async Task EnviarNotificacaoImediataAsync(NotificacaoSgpDto notificacao)
        {

            notificacao.InserirCategoria();
            Dictionary<string, string> dicionarioNotificacao = montarNotificacao(notificacao);

            var notificacaoFirebase = new Notification
            {
                Title = notificacao.Titulo,
                Body = UtilString.EncodeUTF8("Você recebeu uma nova mensagem da SME. Clique aqui para visualizar os detalhes.").Replace("�", "ê"),
            };

            await EnviarNotificacao(notificacao, dicionarioNotificacao, notificacaoFirebase);
        }

        private static Dictionary<string, string> montarNotificacao(NotificacaoSgpDto notificacao)
        {
            var mensagem = Regex.Replace(notificacao.Mensagem, @"<(.|\n)*?>", string.Empty);
            return new Dictionary<String, String>
            {
                ["Titulo"] = notificacao.Titulo,
                ["Mensagem"] = mensagem.Length > 256 ? mensagem.Substring(0, 256) : mensagem,
                ["categoriaNotificacao"] = notificacao.CategoriaNotificacao,
                ["Id"] = notificacao.Id.ToString(),
                ["CriadoEm"] = notificacao.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss.ffffff"),
                ["click_action"] = "FLUTTER_NOTIFICATION_CLICK",
            };
        }

        private async Task EnviarNotificacao(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification notificacaoFirebase)
        {
            switch (notificacao.TipoComunicado)
            {
                case TipoComunicado.SME:
                    await EnviarNotificacaoSME(notificacao.ObterGrupoLista(), dicionarioNotificacao, notificacaoFirebase);
                    break;
                case TipoComunicado.SME_ANO:
                    await EnviarNotificacaoSerieResumida(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    break;
                case TipoComunicado.DRE:
                    await EnviarComunicadoDRE(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    break;
                case TipoComunicado.DRE_ANO:
                    await EnviarComunicadoDRE_ANO(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    break;
                case TipoComunicado.UE:
                    await EnviarComunicadoUE(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    break;
                case TipoComunicado.UEMOD:
                    if (notificacao.ObterSeriesResumidas().Any())
                    {
                        await EnviarNotificacaoSerieResumida(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    }
                    else
                    {
                        await EnviarComunicadoUEModalidade(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    }
                    break;
                case TipoComunicado.TURMA:
                    await EnviarComunicadoTurmas(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    break;
                case TipoComunicado.ALUNO:
                    await EnviarComunicadoAlunos(notificacao, dicionarioNotificacao, notificacaoFirebase);
                    break;
                default:
                    break;
            }
        }

        private async Task EnviarComunicadoAlunos(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            foreach (var aluno in notificacao.Alunos)
            {

                var data = new Dictionary<String, String>(dicionarioNotificacao);

                var topico = "ALU-" + aluno;

                data.Add("CodigoAluno", topico);
                data.Add("CodigoEOL", aluno);

                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }
        }

        private async Task EnviarComunicadoTurmas(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            foreach (var turma in notificacao.Turmas)
            {
                var data = new Dictionary<String, String>(dicionarioNotificacao);
                var topico = "TUR-" + turma;
                data.Add("CodigoTurma", "TUR-" + turma);
                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }
        }

        private async Task EnviarComunicadoUEModalidade(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            var grupos = notificacao.ObterGrupoLista();
            foreach (var grupo in grupos)
            {
                var data = new Dictionary<String, String>(dicionarioNotificacao);
                var topico = "UE-" + notificacao.CodigoUe + "-MOD-" + grupo;
                data.Add("CodigoUe", "UE-" + notificacao.CodigoUe);
                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
            }
        }

        private async Task EnviarComunicadoUE(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            var data = new Dictionary<String, String>(dicionarioNotificacao);
            var topico = "UE-" + notificacao.CodigoUe;
            data.Add("CodigoUe", "UE-" + topico);

            await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
        }

        private async Task EnviarComunicadoDRE(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification Notificacao)
        {
            var data = new Dictionary<String, String>(dicionarioNotificacao);
            var topico = "DRE-" + notificacao.CodigoDre;
            data.Add("CodigoDre", "DRE-" + topico);

            await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, Notificacao, data)));
        }

        private async Task EnviarComunicadoDRE_ANO(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification notificacaoFirebase)
        {
            var data = new Dictionary<String, String>(dicionarioNotificacao);
            var seriesResumidas = notificacao.ObterSeriesResumidas();

            foreach (var serieResumida in seriesResumidas)
            {
                var topico = $"SERIERESUMIDA-{serieResumida}-DRE-{notificacao.CodigoDre}";
                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, notificacaoFirebase, data)));
            }
        }

        private async Task EnviarNotificacaoSME(List<int> grupos, Dictionary<string, string> dicionarioNotificacao, Notification notificacaoFirebase)
        {
            foreach (var grupo in grupos)
            {
                var data = dicionarioNotificacao;

                var topico = "Grupo-" + grupo.ToString();

                await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, notificacaoFirebase, data)));

            }
        }
        private async Task EnviarNotificacaoSerieResumida(NotificacaoSgpDto notificacao, Dictionary<string, string> dicionarioNotificacao, Notification notificacaoFirebase)
        {
            var data = new Dictionary<String, String>(dicionarioNotificacao);
            var grupos = notificacao.ObterGrupoLista();
            var seriesResumidas = notificacao.ObterSeriesResumidas();

            foreach (var serieResumida in seriesResumidas)
            {
                foreach (var grupo in grupos)
                {
                    var topico = $"SERIERESUMIDA-{serieResumida}-MOD-{grupo}";
                    await mediator.Send(new EnviarNotificacaoPorGrupoCommand(MontaMensagem(topico, notificacaoFirebase, data)));
                }
            }
        }

        private static Message MontaMensagem(string topico, Notification notificacao, Dictionary<string, string> data)
        {
            Notification notificacaoUTF8 = MontaNotificacaoUTF8(notificacao);

            var Mensagem = new Message
            {
                Notification = notificacaoUTF8,
                Data = data,
                Topic = topico
            };
            return Mensagem;
        }

        private static Notification MontaNotificacaoUTF8(Notification notificacao)
        {
            Notification notificacaoUTF8 = new Notification
            {
                Title = UtilString.EncodeUTF8(notificacao.Title),
                Body = UtilString.EncodeUTF8(notificacao.Body)
            };
            return notificacaoUTF8;
        }
    }
}