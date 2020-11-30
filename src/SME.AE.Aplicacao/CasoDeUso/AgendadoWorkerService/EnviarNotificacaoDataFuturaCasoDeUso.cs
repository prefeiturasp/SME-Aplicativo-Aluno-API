using AutoMapper;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class EnviarNotificacaoDataFuturaCasoDeUso
    {
        private readonly INotificacaoRepository notificacaoRepository;
        private readonly ICriarNotificacaoUseCase criarNotificacaoUseCase;

        public EnviarNotificacaoDataFuturaCasoDeUso(INotificacaoRepository notificacaoRepository, ICriarNotificacaoUseCase criarNotificacaoUseCase)
        {
            this.notificacaoRepository = notificacaoRepository ?? throw new ArgumentNullException(nameof(notificacaoRepository));
            this.criarNotificacaoUseCase = criarNotificacaoUseCase ?? throw new ArgumentNullException(nameof(criarNotificacaoUseCase));
        }

        public async Task ExecutarAsync()
        {
            var notificacoesParaEnviar = await notificacaoRepository.ListarNotificacoesNaoEnviadas();
            foreach(var notificacao in notificacoesParaEnviar)
            {
                await EnviaPushNotification(notificacao);
                await AlteraFlagEnviado(notificacao);
            }
        }

        private async Task AlteraFlagEnviado(NotificacaoSgpDto notificacao)
        {
            var atualizaDto = new AtualizarNotificacaoDto
            {
                EnviadoPushNotification = true,
                AlteradoEm = DateTime.Now,
                AlteradoPor = "worker",
                DataExpiracao = notificacao.DataExpiracao,
                Id = notificacao.Id,
                Mensagem = notificacao.Mensagem,
                Titulo = notificacao.Titulo
            };
            await notificacaoRepository.Atualizar(atualizaDto);
        }

        private async Task EnviaPushNotification(NotificacaoSgpDto notificacao)
        {
            await criarNotificacaoUseCase.EnviarNotificacaoImediataAsync(notificacao);
        }
    }
}
