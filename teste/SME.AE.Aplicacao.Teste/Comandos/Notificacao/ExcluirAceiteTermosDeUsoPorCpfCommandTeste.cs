using Moq;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.TermosDeUso
{
    public class ExcluirAceiteTermosDeUsoPorCpfCommandTeste
    {
        private readonly Mock<IAceiteTermosDeUsoRepositorio> _repositorioMock;
        private readonly ExcluirAceiteTermosDeUsoPorCpfCommandHandler _handler;

        public ExcluirAceiteTermosDeUsoPorCpfCommandTeste()
        {
            _repositorioMock = new Mock<IAceiteTermosDeUsoRepositorio>();
            _handler = new ExcluirAceiteTermosDeUsoPorCpfCommandHandler(_repositorioMock.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando o repositório for nulo")]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new ExcluirAceiteTermosDeUsoPorCpfCommandHandler(null));

            Assert.Equal("repositorio", excecao.ParamName);
        }

        [Fact(DisplayName = "Deve excluir aceite dos termos de uso por CPF com sucesso e retornar true")]
        public async Task Handle_CpfValido_DeveExcluirLogicamenteERetornarTrue()
        {
            // Arrange
            var cpf = "12345678901";
            var command = new ExcluirAceiteTermosDeUsoPorCpfCommand(cpf);

            _repositorioMock
                .Setup(r => r.ExcluirLogicamentePorCpf(cpf))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _repositorioMock.Verify(r => r.ExcluirLogicamentePorCpf(cpf), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar false quando a exclusão lógica não afetar registros")]
        public async Task Handle_QuandoRepositorioRetornaFalse_DeveRetornarFalse()
        {
            // Arrange
            var cpf = "12345678901";
            var command = new ExcluirAceiteTermosDeUsoPorCpfCommand(cpf);

            _repositorioMock
                .Setup(r => r.ExcluirLogicamentePorCpf(cpf))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            _repositorioMock.Verify(r => r.ExcluirLogicamentePorCpf(cpf), Times.Once);
        }
    }
}