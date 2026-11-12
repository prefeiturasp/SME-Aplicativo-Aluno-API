using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SME.AE.Aplicacao.Comandos.Usuario.ReiniciarSenha;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class ReiniciarSenhaCommandTeste
    {
        private readonly Mock<IUsuarioRepository> repositorio;
        private readonly ReiniciarSenhaCommandHandler handler;

        public ReiniciarSenhaCommandTeste()
        {
            repositorio = new Mock<IUsuarioRepository>();
            handler = new ReiniciarSenhaCommandHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_ComRepositorioNulo_DeveLancarExcecao()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ReiniciarSenhaCommandHandler(null));

            Assert.Equal("usuarioRepository", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_ComUsuarioEncontrado_DeveAtualizarPrimeiroAcesso()
        {
            const long id = 15;
            var usuario = new global::SME.AE.Dominio.Entidades.Usuario { Id = id, PrimeiroAcesso = false };
            var command = new ReiniciarSenhaCommand { Id = id, PrimeiroAcesso = true };
            repositorio.Setup(x => x.ObterPorIdAsync(id)).ReturnsAsync(usuario);

            var resultado = await handler.Handle(command, CancellationToken.None);

            Assert.Equal(default, resultado);
            Assert.True(usuario.PrimeiroAcesso);
            repositorio.Verify(x => x.AtualizarPrimeiroAcesso(id, true), Times.Once);
        }

        [Fact]
        public async Task Handle_ComUsuarioNaoEncontrado_DeveLancarNegocioExceptionESemAtualizar()
        {
            const long id = 99;
            repositorio.Setup(x => x.ObterPorIdAsync(id)).ReturnsAsync((global::SME.AE.Dominio.Entidades.Usuario)null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                handler.Handle(new ReiniciarSenhaCommand { Id = id, PrimeiroAcesso = true }, CancellationToken.None));

            Assert.Contains(id.ToString(), excecao.Message);
            repositorio.Verify(x => x.AtualizarPrimeiroAcesso(It.IsAny<long>(), It.IsAny<bool>()), Times.Never);
        }
    }
}
