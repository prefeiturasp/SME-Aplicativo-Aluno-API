using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.EnviarNotificacaoPorGrupo;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Notificacao
{
    public class CriarNotificacaoUseCaseTeste : BaseTeste
    {
        private readonly ICriarNotificacaoUseCase criarNotificacaoUseCase;

        public CriarNotificacaoUseCaseTeste()
        {
            criarNotificacaoUseCase = new CriarNotificacaoUseCase(mediator.Object, mapper.Object);
        }

        private void InstanciarMediator()
        {
            mediator.Setup(x => x.Send(It.IsAny<EnviarNotificacaoPorGrupoCommand>(), It.IsAny<CancellationToken>()));
            mediator.Setup(x => x.Send(It.IsAny<EnviarNotificacaoPorGrupoCommand>(), It.IsAny<CancellationToken>()));
        }

        [Fact(DisplayName = "Deve criar as notificações SME")]
        public async Task ValidarEnvioNotificacaoSME()
        {
            InstanciarMediator();

            await criarNotificacaoUseCase.Executar(NotificacaoSME);
        }

        [Fact(DisplayName = "Deve criar as notificações DRE")]
        public async Task ValidarEnvioNotificacaoDRE()
        {
            InstanciarMediator();

            await criarNotificacaoUseCase.Executar(NotificacaoDRE);
        }

        [Fact(DisplayName = "Deve criar as notificações UE")]
        public async Task ValidarEnvioNotificacaoUE()
        {
            InstanciarMediator();

            await criarNotificacaoUseCase.Executar(NotificacaoUE);
        }

        [Fact(DisplayName = "Deve criar as notificações UE MOD")]
        public async Task ValidarEnvioNotificacaoUEMOD()
        {
            InstanciarMediator();

            await criarNotificacaoUseCase.Executar(NotificacaoUEMOD);
        }

        [Fact(DisplayName = "Deve criar as notificações TURMA")]
        public async Task ValidarEnvioNotificacaoTURMA()
        {
            InstanciarMediator();

            await criarNotificacaoUseCase.Executar(NotificacaoTurma);
        }

        [Fact(DisplayName = "Deve criar as notificações ALUNO")]
        public async Task ValidarEnvioNotificacaoALUNO()
        {
            InstanciarMediator();

            await criarNotificacaoUseCase.Executar(NotificacaoAluno);
        }

        private NotificacaoSgpDto NotificacaoSME => new NotificacaoSgpDto
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.SME,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            Grupo = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoDRE => new NotificacaoSgpDto
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.DRE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoUE => new NotificacaoSgpDto
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoUEMOD => new NotificacaoSgpDto
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UEMOD,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Grupo = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoTurma => new NotificacaoSgpDto
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.TURMA,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Grupo = "1,2,3,4,5,6",
            Turmas = new List<string> { "0000000" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private NotificacaoSgpDto NotificacaoAluno => new NotificacaoSgpDto
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.ALUNO,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Grupo = "1,2,3,4,5,6",
            Turmas = new List<string> { "0000000" },
            Alunos = new List<string> { "0000000" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };
    }
}
