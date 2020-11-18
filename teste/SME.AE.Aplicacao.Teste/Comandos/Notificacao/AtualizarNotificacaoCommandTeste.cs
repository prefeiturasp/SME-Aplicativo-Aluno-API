//using Moq;
//using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
//using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
//using SME.AE.Dominio.Comum.Enumeradores;
//using SME.AE.Dominio.Entidades;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace SME.AE.Aplicacao.Teste.Comandos
//{
//    public class AtualizarNotificacaoCommandTeste
//    {
//        private Mock<INotificacaoRepository> repository;
//        private AtualizarNotificacaoCommandHandler handler;

//        public AtualizarNotificacaoCommandTeste()
//        {
//            repository = new Mock<INotificacaoRepository>();
//            handler = new AtualizarNotificacaoCommandHandler(repository.Object);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao SME")]
//        public async Task Deve_Atualizar_Notificacao_SME()
//        {
//            InstanciarSetup();

//            await handler.Handle(new AtualizarNotificacaoCommand(NotificacaoSME), new CancellationToken());
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao DRE")]
//        public async Task Deve_Atualizar_Notificacao_DRE()
//        {
//            InstanciarSetup();

//            await handler.Handle(new AtualizarNotificacaoCommand(NotificacaoDRE), new CancellationToken());
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao UE")]
//        public async Task Deve_Atualizar_Notificacao_UE()
//        {
//            InstanciarSetup();

//            await handler.Handle(new AtualizarNotificacaoCommand(NotificacaoUE), new CancellationToken());
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao UEMOD")]
//        public async Task Deve_Atualizar_Notificacao_UEMOD()
//        {
//            InstanciarSetup();

//            await handler.Handle(new AtualizarNotificacaoCommand(NotificacaoUEMOD), new CancellationToken());
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao TURMA")]
//        public async Task Deve_Atualizar_Notificacao_TURMA()
//        {
//            InstanciarSetup();

//            await handler.Handle(new AtualizarNotificacaoCommand(NotificacaoTurma), new CancellationToken());
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao ALUNO")]
//        public async Task Deve_Atualizar_Notificacao_ALUNO()
//        {
//            InstanciarSetup();

//            await handler.Handle(new AtualizarNotificacaoCommand(NotificacaoAluno), new CancellationToken());
//        }

//        private void InstanciarSetup()
//        {
//            repository.Setup(x => x.Atualizar(It.IsAny<Notificacao>()));
//        }

//        private Notificacao NotificacaoSME => new Notificacao
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.SME,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            Grupo = "1,2,3,4,5,6",
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };

//        private Notificacao NotificacaoDRE => new Notificacao
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.DRE,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            CodigoDre = "000000",
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };

//        private Notificacao NotificacaoUE => new Notificacao
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.UE,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            CodigoDre = "0000000",
//            CodigoUe = "0000000",
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };

//        private Notificacao NotificacaoUEMOD => new Notificacao
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.UEMOD,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            CodigoDre = "0000000",
//            CodigoUe = "0000000",
//            Grupo = "1,2,3,4,5,6",
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };

//        private Notificacao NotificacaoTurma => new Notificacao
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.TURMA,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            CodigoDre = "0000000",
//            CodigoUe = "0000000",
//            Grupo = "1,2,3,4,5,6",
//            Turmas = new List<string> { "0000000" },
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };

//        private Notificacao NotificacaoAluno => new Notificacao
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.ALUNO,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            CodigoDre = "0000000",
//            CodigoUe = "0000000",
//            Grupo = "1,2,3,4,5,6",
//            Turmas = new List<string> { "0000000" },
//            Alunos = new List<string> { "0000000" },
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };
//    }
//}
