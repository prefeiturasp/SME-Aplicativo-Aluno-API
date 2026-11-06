using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.Criar;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Comum.Enumeradores;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class CriarNotificacaoCommandTeste
    {
        private readonly Mock<INotificacaoRepositorio> notificacaoRepository;
        private readonly CriarNotificacaoCommandHandler handler;

        public CriarNotificacaoCommandTeste()
        {
            notificacaoRepository = new Mock<INotificacaoRepositorio>();
            handler = new CriarNotificacaoCommandHandler(notificacaoRepository.Object);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo SME")]
        public async Task Deve_Criar_Notificacao_SME()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoSME;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.SME, resultado.TipoComunicado);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.SME)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()), Times.Never);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()), Times.Never);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo DRE")]
        public async Task Deve_Criar_Notificacao_DRE()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoDRE;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.DRE, resultado.TipoComunicado);
            Assert.Equal("000000", resultado.CodigoDre);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.DRE)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()), Times.Never);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()), Times.Never);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo UE")]
        public async Task Deve_Criar_Notificacao_UE()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoUE;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.UE, resultado.TipoComunicado);
            Assert.Equal("0000000", resultado.CodigoUe);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.UE)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()), Times.Never);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()), Times.Never);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo UEMOD")]
        public async Task Deve_Criar_Notificacao_UEMOD()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoUEMOD;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.UEMOD, resultado.TipoComunicado);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.UEMOD)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()), Times.Never);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()), Times.Never);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo TURMA e vincular as turmas")]
        public async Task Deve_Criar_Notificacao_TURMA()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoTurma;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.TURMA, resultado.TipoComunicado);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.TURMA)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.Is<NotificacaoTurma>(t => t.NotificacaoId == notificacao.Id && t.CodigoTurma == 12345)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()), Times.Never);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo ALUNO e vincular os alunos")]
        public async Task Deve_Criar_Notificacao_ALUNO()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoAluno;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.ALUNO, resultado.TipoComunicado);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.ALUNO)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.Is<NotificacaoAluno>(a => a.NotificacaoId == notificacao.Id && a.CodigoAluno == 54321)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()), Times.Never);
        }

        [Fact(DisplayName = "Deve Criar a Notificação do tipo MENSAGEM_AUTOMATICA e vincular os alunos")]
        public async Task Deve_Criar_Notificacao_MENSAGEM_AUTOMATICA()
        {
            // Arrange
            InstanciarSetup();
            var notificacao = NotificacaoMensagemAutomatica;

            // Act
            var resultado = await handler.Handle(new CriarNotificacaoCommand(notificacao), CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal(TipoComunicado.MENSAGEM_AUTOMATICA, resultado.TipoComunicado);
            notificacaoRepository.Verify(x => x.Criar(It.Is<Dominio.Entidades.Notificacao>(n => n.TipoComunicado == TipoComunicado.MENSAGEM_AUTOMATICA)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoAluno(It.Is<NotificacaoAluno>(a => a.NotificacaoId == notificacao.Id && a.CodigoAluno == 54321)), Times.Once);
            notificacaoRepository.Verify(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()), Times.Never);
        }

        [Fact(DisplayName = "Deve propagar exceção quando ocorrer erro ao criar notificação")]
        public async Task Deve_Propagar_Excecao_Quando_Falhar_Ao_Criar()
        {
            // Arrange
            notificacaoRepository
                .Setup(x => x.Criar(It.IsAny<Dominio.Entidades.Notificacao>()))
                .ThrowsAsync(new InvalidOperationException("Falha simulada no banco"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new CriarNotificacaoCommand(NotificacaoSME), CancellationToken.None));
        }

        private void InstanciarSetup()
        {
            notificacaoRepository
                .Setup(x => x.Criar(It.IsAny<Dominio.Entidades.Notificacao>()))
                .ReturnsAsync((Dominio.Entidades.Notificacao n) => n);

            notificacaoRepository
                .Setup(x => x.InserirNotificacaoAluno(It.IsAny<NotificacaoAluno>()))
                .Returns(Task.CompletedTask);

            notificacaoRepository
                .Setup(x => x.InserirNotificacaoTurma(It.IsAny<NotificacaoTurma>()))
                .Returns(Task.CompletedTask);
        }

        private Dominio.Entidades.Notificacao NotificacaoSME => new Dominio.Entidades.Notificacao
        {
            Id = 1,
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.SME,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            Modalidades = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Dominio.Entidades.Notificacao NotificacaoDRE => new Dominio.Entidades.Notificacao
        {
            Id = 2,
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.DRE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Dominio.Entidades.Notificacao NotificacaoUE => new Dominio.Entidades.Notificacao
        {
            Id = 3,
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UE,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Dominio.Entidades.Notificacao NotificacaoUEMOD => new Dominio.Entidades.Notificacao
        {
            Id = 4,
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.UEMOD,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Modalidades = "1,2,3,4,5,6",
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Dominio.Entidades.Notificacao NotificacaoTurma => new Dominio.Entidades.Notificacao
        {
            Id = 5,
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.TURMA,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Modalidades = "1,2,3,4,5,6",
            Turmas = new List<string> { "12345" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Dominio.Entidades.Notificacao NotificacaoAluno => new Dominio.Entidades.Notificacao
        {
            Id = 6,
            Titulo = "Teste",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.ALUNO,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem",
            CodigoDre = "0000000",
            CodigoUe = "0000000",
            Modalidades = "1,2,3,4,5,6",
            Turmas = new List<string> { "12345" },
            Alunos = new List<string> { "54321" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };

        private Dominio.Entidades.Notificacao NotificacaoMensagemAutomatica => new Dominio.Entidades.Notificacao
        {
            Id = 7,
            Titulo = "Teste Automático",
            AnoLetivo = DateTime.Now.Year,
            TipoComunicado = TipoComunicado.MENSAGEM_AUTOMATICA,
            DataEnvio = DateTime.Now,
            Mensagem = "Teste Mensagem Automática",
            Alunos = new List<string> { "54321" },
            DataExpiracao = DateTime.Now.AddDays(7)
        };
    }
}