using Moq;
using SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.Autenticacao.CriarUsuario
{
    public class RegistrarAceiteDosTermosDeUsoCommandTeste
    {
        private readonly Mock<IAceiteTermosDeUsoRepositorio> _repositorioMock;
        private readonly RegistrarAceiteDosTermosDeUsoCommandHandler _handler;

        public RegistrarAceiteDosTermosDeUsoCommandTeste()
        {
            _repositorioMock = new Mock<IAceiteTermosDeUsoRepositorio>();
            _handler = new RegistrarAceiteDosTermosDeUsoCommandHandler(_repositorioMock.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando o repositório for nulo")]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new RegistrarAceiteDosTermosDeUsoCommandHandler(null));

            Assert.Equal("aceiteTermosDeUsorepositorio", excecao.ParamName);
        }

        [Fact(DisplayName = "Deve registrar o aceite dos termos de uso com sucesso e retornar true")]
        public async Task Handle_DadosValidos_DeveRegistrarAceiteERetornarTrue()
        {
            // Arrange
            var command = CriarCommandPadrao();

            _repositorioMock
                .Setup(r => r.RegistrarAceite(It.IsAny<AceiteTermosDeUso>()))
                .ReturnsAsync(true);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(resultado);
            _repositorioMock.Verify(r => r.RegistrarAceite(It.Is<AceiteTermosDeUso>(a =>
                a.TermosDeUsoId == command.TermoDeUsoId &&
                a.CpfUsuario == command.Usuario &&
                a.Device == command.Device &&
                a.Ip == command.Ip &&
                a.Versao.Equals(command.Versao) &&
                a.Hash != null &&
                a.Hash.Length > 0 &&
                !a.Excluido
            )), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar false quando o repositório falhar ao registrar o aceite")]
        public async Task Handle_QuandoRepositorioRetornaFalse_DeveRetornarFalse()
        {
            // Arrange
            var command = CriarCommandPadrao();

            _repositorioMock
                .Setup(r => r.RegistrarAceite(It.IsAny<AceiteTermosDeUso>()))
                .ReturnsAsync(false);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(resultado);
            _repositorioMock.Verify(r => r.RegistrarAceite(It.IsAny<AceiteTermosDeUso>()), Times.Once);
        }

        private static RegistrarAceiteDosTermosDeUsoCommand CriarCommandPadrao()
        {
            return new RegistrarAceiteDosTermosDeUsoCommand(
                termoDeUsoId: 1L,
                usuario: "12345678901",
                device: "Android 14",
                ip: "192.168.0.1",
                versao: 1.0
            );
        }
    }
}