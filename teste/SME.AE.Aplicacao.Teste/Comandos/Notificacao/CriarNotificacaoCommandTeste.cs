using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class CriarNotificacaoCommandTeste
    {
        public Mock<INotificacaoRepository> notificacaoRepository;
        public CriarNotificacaoCommandHandler handler;

        public CriarNotificacaoCommandTeste()
        {
            notificacaoRepository = new Mock<INotificacaoRepository>();
            handler = new CriarNotificacaoCommandHandler(notificacaoRepository.Object);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo SME")]
        public async Task Deve_Criar_Notificacao_SME()
        {
            InstanciarSetup();

            await handler.Handle(new CriarNotificacaoCommand(NotificacaoSME), new CancellationToken());
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo DRE")]
        public async Task Deve_Criar_Notificacao_DRE()
        {
            InstanciarSetup();

            await handler.Handle(new CriarNotificacaoCommand(NotificacaoDRE), new CancellationToken());
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo UE")]
        public async Task Deve_Criar_Notificacao_UE()
        {
            InstanciarSetup();

            await handler.Handle(new CriarNotificacaoCommand(NotificacaoUE), new CancellationToken());
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo UEMOD")]
        public async Task Deve_Criar_Notificacao_UEMOD()
        {
            InstanciarSetup();

            await handler.Handle(new CriarNotificacaoCommand(NotificacaoUEMOD), new CancellationToken());
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo TURMA")]
        public async Task Deve_Criar_Notificacao_TURMA()
        {
            InstanciarSetup();

            await handler.Handle(new CriarNotificacaoCommand(NotificacaoTurma), new CancellationToken());
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo ALUNO")]
        public async Task Deve_Criar_Notificacao_ALUNO()
        {
            InstanciarSetup();

            await handler.Handle(new CriarNotificacaoCommand(NotificacaoAluno), new CancellationToken());
        }
      

        private void InstanciarSetup()
        {
            notificacaoRepository.Setup(x => x.Criar(It.IsAny<Notificacao>()));
            notificacaoRepository.Setup(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()));
            notificacaoRepository.Setup(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()));
        }

        private Notificacao NotificacaoSME => new Notificacao
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.SME,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            Grupo = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Notificacao NotificacaoDRE => new Notificacao
        {
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.DRE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Notificacao NotificacaoUE => new Notificacao
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

        private Notificacao NotificacaoUEMOD => new Notificacao
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

        private Notificacao NotificacaoTurma => new Notificacao
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

        private Notificacao NotificacaoAluno => new Notificacao
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
