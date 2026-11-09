using Moq;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.TermoDeUso
{
    public class ExcluirAceiteTermosDeUsoPorCpfCommandTeste
    {
        private readonly Mock<IAceiteTermosDeUsoRepositorio> repositorio;
        private readonly ExcluirAceiteTermosDeUsoPorCpfCommandHandler handler;

        public ExcluirAceiteTermosDeUsoPorCpfCommandTeste()
        {
            repositorio = new Mock<IAceiteTermosDeUsoRepositorio>();
            handler = new ExcluirAceiteTermosDeUsoPorCpfCommandHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new ExcluirAceiteTermosDeUsoPorCpfCommandHandler(null));

            Assert.Equal("repositorio", excecao.ParamName);
        }

        [Fact]
        public void Construtor_DeveAtribuirCpf()
        {
            var command = new ExcluirAceiteTermosDeUsoPorCpfCommand("12345678901");

            Assert.Equal("12345678901", command.CpfUsuario);
        }

        [Fact]
        public async Task Handle_CpfValido_DeveExcluirLogicamenteERetornarTrue()
        {
            const string cpf = "12345678901";
            repositorio.Setup(x => x.ExcluirLogicamentePorCpf(cpf)).ReturnsAsync(true);

            var resultado = await handler.Handle(
                new ExcluirAceiteTermosDeUsoPorCpfCommand(cpf), CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.ExcluirLogicamentePorCpf(cpf), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioRetornaFalse_DeveRetornarFalse()
        {
            const string cpf = "12345678901";
            repositorio.Setup(x => x.ExcluirLogicamentePorCpf(cpf)).ReturnsAsync(false);

            var resultado = await handler.Handle(
                new ExcluirAceiteTermosDeUsoPorCpfCommand(cpf), CancellationToken.None);

            Assert.False(resultado);
            repositorio.Verify(x => x.ExcluirLogicamentePorCpf(cpf), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioFalhar_DevePropagarExcecao()
        {
            repositorio.Setup(x => x.ExcluirLogicamentePorCpf(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("falha ao excluir aceite"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new ExcluirAceiteTermosDeUsoPorCpfCommand("12345678901"), CancellationToken.None));

            Assert.Equal("falha ao excluir aceite", excecao.Message);
        }
    }
}
