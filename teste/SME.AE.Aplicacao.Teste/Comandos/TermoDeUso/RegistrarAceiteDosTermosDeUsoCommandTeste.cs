using Moq;
using SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.TermoDeUso
{
    public class RegistrarAceiteDosTermosDeUsoCommandTeste
    {
        private readonly Mock<IAceiteTermosDeUsoRepositorio> repositorio;
        private readonly RegistrarAceiteDosTermosDeUsoCommandHandler handler;

        public RegistrarAceiteDosTermosDeUsoCommandTeste()
        {
            repositorio = new Mock<IAceiteTermosDeUsoRepositorio>();
            handler = new RegistrarAceiteDosTermosDeUsoCommandHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new RegistrarAceiteDosTermosDeUsoCommandHandler(null));

            Assert.Equal("aceiteTermosDeUsorepositorio", excecao.ParamName);
        }

        [Fact]
        public void Construtor_DeveAtribuirTodosOsDados()
        {
            var command = new RegistrarAceiteDosTermosDeUsoCommand(1, "12345678901", "Android", "192.168.0.1", 1.0);

            Assert.Equal(1, command.TermoDeUsoId);
            Assert.Equal("12345678901", command.Usuario);
            Assert.Equal("Android", command.Device);
            Assert.Equal("192.168.0.1", command.Ip);
            Assert.Equal(1.0, command.Versao);
        }

        [Fact]
        public async Task Handle_DadosValidos_DeveRegistrarAceiteERetornarTrue()
        {
            var command = CriarCommandPadrao();
            repositorio.Setup(x => x.RegistrarAceite(It.IsAny<AceiteTermosDeUso>())).ReturnsAsync(true);

            var resultado = await handler.Handle(command, CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.RegistrarAceite(It.Is<AceiteTermosDeUso>(aceite =>
                aceite.TermosDeUsoId == command.TermoDeUsoId &&
                aceite.CpfUsuario == command.Usuario &&
                aceite.Device == command.Device &&
                aceite.Ip == command.Ip &&
                aceite.Versao == command.Versao &&
                aceite.Hash != null &&
                aceite.Hash.Length > 0)), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioRetornaFalse_DeveRetornarFalse()
        {
            repositorio.Setup(x => x.RegistrarAceite(It.IsAny<AceiteTermosDeUso>())).ReturnsAsync(false);

            var resultado = await handler.Handle(CriarCommandPadrao(), CancellationToken.None);

            Assert.False(resultado);
            repositorio.Verify(x => x.RegistrarAceite(It.IsAny<AceiteTermosDeUso>()), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioFalhar_DevePropagarExcecao()
        {
            repositorio.Setup(x => x.RegistrarAceite(It.IsAny<AceiteTermosDeUso>()))
                .ThrowsAsync(new InvalidOperationException("falha ao registrar aceite"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(CriarCommandPadrao(), CancellationToken.None));

            Assert.Equal("falha ao registrar aceite", excecao.Message);
        }

        private static RegistrarAceiteDosTermosDeUsoCommand CriarCommandPadrao()
        {
            return new RegistrarAceiteDosTermosDeUsoCommand(1, "12345678901", "Android 14", "192.168.0.1", 1.0);
        }
    }
}
