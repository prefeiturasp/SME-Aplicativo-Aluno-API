using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SME.AE.Aplicacao.Comandos.Notificacao.Remover;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using Xunit;
using NotificacaoEntidade = SME.AE.Dominio.Entidades.Notificacao;

namespace SME.AE.Aplicacao.Teste.Comandos.Notificacao
{
    public class RemoverNotificacaoCommandTeste
    {
        [Fact]
        public async Task Handle_ComNotificacaoNaoEncontrada_DeveRetornarErroENaoRemover()
        {
            var repositorio = new Mock<INotificacaoRepositorio>();
            repositorio.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync((NotificacaoEntidade)null);
            var handler = new RemoverNotificacaoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoCommand([1L]), CancellationToken.None);

            Assert.Equal("1 - Notificação não encontrada", resultado[0]);
            repositorio.Verify(x => x.Remover(It.IsAny<NotificacaoEntidade>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComNotificacaoEncontradaEExclusaoComSucesso_DeveRetornarArraySemErro()
        {
            var repositorio = new Mock<INotificacaoRepositorio>();
            var notificacao = new NotificacaoEntidade { Id = 1, Titulo = "Título" };
            repositorio.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(notificacao);
            repositorio.Setup(x => x.Remover(notificacao)).ReturnsAsync(true);
            var handler = new RemoverNotificacaoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoCommand([1L]), CancellationToken.None);

            Assert.Null(resultado[0]);
            repositorio.Verify(x => x.Remover(notificacao), Times.Once);
        }

        [Fact]
        public async Task Handle_ComFalhaNaExclusao_DeveCapturarExcecaoESeguirProcessamento()
        {
            var repositorio = new Mock<INotificacaoRepositorio>();
            var notificacao = new NotificacaoEntidade { Id = 1, Titulo = "Título" };
            repositorio.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(notificacao);
            repositorio.Setup(x => x.Remover(notificacao)).ThrowsAsync(new InvalidOperationException());
            var handler = new RemoverNotificacaoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoCommand([1L]), CancellationToken.None);

            Assert.Null(resultado[0]);
        }
    }

    public class RemoverNotificacaoPorIdCommandTeste
    {
        [Fact]
        public async Task Handle_ComNotificacaoNaoEncontrada_DeveRetornarFalso()
        {
            var repositorio = new Mock<INotificacaoRepositorio>();
            repositorio.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync((NotificacaoEntidade)null);
            var handler = new RemoverNotificacaoPorIdCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoPorIdCommand(1), CancellationToken.None);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Handle_ComRemocaoBemSucedida_DeveRetornarVerdadeiro()
        {
            var repositorio = new Mock<INotificacaoRepositorio>();
            var notificacao = new NotificacaoEntidade { Id = 1 };
            repositorio.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(notificacao);
            repositorio.Setup(x => x.Remover(notificacao)).ReturnsAsync(true);
            var handler = new RemoverNotificacaoPorIdCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoPorIdCommand(1), CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.Remover(notificacao), Times.Once);
        }

        [Fact]
        public async Task Handle_ComExcecaoNaRemocao_DeveRetornarFalso()
        {
            var repositorio = new Mock<INotificacaoRepositorio>();
            var notificacao = new NotificacaoEntidade { Id = 1 };
            repositorio.Setup(x => x.ObterPorIdAsync(1)).ReturnsAsync(notificacao);
            repositorio.Setup(x => x.Remover(notificacao)).ThrowsAsync(new InvalidOperationException());
            var handler = new RemoverNotificacaoPorIdCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoPorIdCommand(1), CancellationToken.None);

            Assert.False(resultado);
        }
    }

    public class RemoverNotificacaoAlunoPorNotificacoesIdsCommandTeste
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_DeveDelegarIdsERetornarResultado(bool resposta)
        {
            var repositorio = new Mock<INotificacaoAlunoRepositorio>();
            long[] ids = [1L, 2L];
            repositorio.Setup(x => x.RemoverPorNotificacoesIds(ids)).ReturnsAsync(resposta);
            var handler = new RemoverNotificacaoAlunoPorNotificacoesIdsCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoAlunoPorNotificacoesIdsCommand(ids), CancellationToken.None);

            Assert.Equal(resposta, resultado);
            repositorio.Verify(x => x.RemoverPorNotificacoesIds(ids), Times.Once);
        }

        [Fact]
        public void Construtor_ComRepositorioNulo_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentNullException>(() => new RemoverNotificacaoAlunoPorNotificacoesIdsCommandHandler(null));
        }
    }

    public class RemoverNotificacaoTurmaPorNotificacoesIdsCommandTeste
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_DeveDelegarIdsERetornarResultado(bool resposta)
        {
            var repositorio = new Mock<INotificacaoTurmaRepositorio>();
            long[] ids = [1L, 2L];
            repositorio.Setup(x => x.RemoverPorNotificacoesIds(ids)).ReturnsAsync(resposta);
            var handler = new RemoverNotificacaoTurmaPorNotificacoesIdsCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoTurmaPorNotificacoesIdsCommand(ids), CancellationToken.None);

            Assert.Equal(resposta, resultado);
            repositorio.Verify(x => x.RemoverPorNotificacoesIds(ids), Times.Once);
        }

        [Fact]
        public void Construtor_ComRepositorioNulo_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentNullException>(() => new RemoverNotificacaoTurmaPorNotificacoesIdsCommandHandler(null));
        }
    }

    public class RemoverNotificacaoUsuarioCommandTeste
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Handle_DeveDelegarIdsERetornarResultado(bool resposta)
        {
            var repositorio = new Mock<IUsuarioNotificacaoRepositorio>();
            long[] ids = [1L, 2L];
            repositorio.Setup(x => x.RemoverPorNotificacoesIds(ids)).ReturnsAsync(resposta);
            var handler = new RemoverNotificacaoUsuarioCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoverNotificacaoUsuarioCommand(ids), CancellationToken.None);

            Assert.Equal(resposta, resultado);
            repositorio.Verify(x => x.RemoverPorNotificacoesIds(ids), Times.Once);
        }
    }
}
