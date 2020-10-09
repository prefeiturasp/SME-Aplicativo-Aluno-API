//using Moq;
//using SME.AE.Aplicacao.Comandos.Notificacao.Atualizar;
//using SME.AE.Aplicacao.Comum.Enumeradores;
//using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
//using SME.AE.Aplicacao.Comum.Modelos;
//using System;
//using System.Collections.Generic;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;

//namespace SME.AE.Aplicacao.Teste.CasosDeUso.Notificacao
//{
//    public class AtualizarNotificacaoUseCaseTeste : BaseTeste
//    {
//        private IAtualizarNotificacaoUseCase atualizarNotificacaoUseCase;

//        public AtualizarNotificacaoUseCaseTeste()
//        {
//            atualizarNotificacaoUseCase = new AtualizarNotificacaoUseCase(mediator.Object, mapper.Object);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao SME")]
//        public async Task DeveAtualizarNotificacaoSME()
//        {
//            InstanciarSetup();

//            await atualizarNotificacaoUseCase.Executar(NotificacaoSME);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao DRE")]
//        public async Task DeveAtualizarNotificacaoDRE()
//        {
//            InstanciarSetup();

//            await atualizarNotificacaoUseCase.Executar(NotificacaoDRE);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao UE")]
//        public async Task DeveAtualizarNotificacaoUE()
//        {
//            InstanciarSetup();

//            await atualizarNotificacaoUseCase.Executar(NotificacaoUE);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao UEMOD")]
//        public async Task DeveAtualizarNotificacaoUEMOD()
//        {
//            InstanciarSetup();

//            await atualizarNotificacaoUseCase.Executar(NotificacaoUEMOD);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao TURMA")]
//        public async Task DeveAtualizarNotificacaoTURMA()
//        {
//            InstanciarSetup();

//            await atualizarNotificacaoUseCase.Executar(NotificacaoTurma);
//        }

//        [Fact(DisplayName = "Deve Atualizar Notificacao ALUNO")]
//        public async Task DeveAtualizarNotificacaoALUNO()
//        {
//            InstanciarSetup();

//            await atualizarNotificacaoUseCase.Executar(NotificacaoAluno);
//        }

//        private void InstanciarSetup()
//        {
//            mediator.Setup(x => x.Send(It.IsAny<AtualizarNotificacaoCommand>(), It.IsAny<CancellationToken>()));
//        }

//        private NotificacaoSgpDto NotificacaoSME => new NotificacaoSgpDto
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.SME,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            Grupo = "1,2,3,4,5,6",
//            DataExpiracao = DateTime.Now.AddDays(7),
//            CriadoPor = "Sistema"
//        };

//        private NotificacaoSgpDto NotificacaoDRE => new NotificacaoSgpDto
//        {
//            Titulo = "Teste",
//            AnoLetivo = DateTime.Now.Year,
//            TipoComunicado = TipoComunicado.DRE,
//            DataEnvio = DateTime.Now,
//            Mensagem = "Teste Mensagem",
//            CodigoDre = "000000",
//            DataExpiracao = DateTime.Now.AddDays(7)
//        };

//        private NotificacaoSgpDto NotificacaoUE => new NotificacaoSgpDto
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

//        private NotificacaoSgpDto NotificacaoUEMOD => new NotificacaoSgpDto
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

//        private NotificacaoSgpDto NotificacaoTurma => new NotificacaoSgpDto
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

//        private NotificacaoSgpDto NotificacaoAluno => new NotificacaoSgpDto
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
