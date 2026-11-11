using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using SME.AE.Aplicacao.Comandos.Autenticacao;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class DispositivoCommandTeste
    {
        [Fact]
        public async Task Remove_Handle_ComUsuarioNaoEncontrado_DeveRetornarFalso()
        {
            var repositorio = new Mock<IUsuarioRepository>();
            repositorio.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf("123")).ReturnsAsync((global::SME.AE.Dominio.Entidades.Usuario)null);
            var handler = new RemoveUsuarioDispositivoCommand.RemoveUsuarioDispositivoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoveUsuarioDispositivoCommand("123", "device"), CancellationToken.None);

            Assert.False(resultado);
            repositorio.Verify(x => x.RemoveUsuarioDispositivo(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Remove_Handle_ComUsuarioEncontrado_DeveDelegarRemocaoERetornarResultado()
        {
            var repositorio = new Mock<IUsuarioRepository>();
            repositorio.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf("123")).ReturnsAsync(new global::SME.AE.Dominio.Entidades.Usuario { Id = 7 });
            repositorio.Setup(x => x.RemoveUsuarioDispositivo(7, "device")).ReturnsAsync(true);
            var handler = new RemoveUsuarioDispositivoCommand.RemoveUsuarioDispositivoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new RemoveUsuarioDispositivoCommand("123", "device"), CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.RemoveUsuarioDispositivo(7, "device"), Times.Once);
        }

        [Fact]
        public async Task Usuario_Handle_ComUsuarioNaoEncontrado_DeveRetornarFalso()
        {
            var repositorio = new Mock<IUsuarioRepository>();
            repositorio.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf("123")).ReturnsAsync((global::SME.AE.Dominio.Entidades.Usuario)null);
            var handler = new UsuarioDispositivoCommand.UsuarioDispositivoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new UsuarioDispositivoCommand("123", "device"), CancellationToken.None);

            Assert.False(resultado);
            repositorio.Verify(x => x.ExisteUsuarioDispositivo(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Usuario_Handle_ComDispositivoExistente_DeveRetornarVerdadeiroSemCriar()
        {
            var repositorio = new Mock<IUsuarioRepository>();
            repositorio.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf("123")).ReturnsAsync(new global::SME.AE.Dominio.Entidades.Usuario { Id = 7 });
            repositorio.Setup(x => x.ExisteUsuarioDispositivo(7, "device")).ReturnsAsync(true);
            var handler = new UsuarioDispositivoCommand.UsuarioDispositivoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new UsuarioDispositivoCommand("123", "device"), CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.CriaUsuarioDispositivo(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Usuario_Handle_ComDispositivoInexistente_DeveCriarEDarSucesso()
        {
            var repositorio = new Mock<IUsuarioRepository>();
            repositorio.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf("123")).ReturnsAsync(new global::SME.AE.Dominio.Entidades.Usuario { Id = 7 });
            repositorio.Setup(x => x.ExisteUsuarioDispositivo(7, "device")).ReturnsAsync(false);
            var handler = new UsuarioDispositivoCommand.UsuarioDispositivoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new UsuarioDispositivoCommand("123", "device"), CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.CriaUsuarioDispositivo(7, "device"), Times.Once);
        }

        [Fact]
        public async Task Usuario_Handle_ComExcecaoNaDependencia_DeveRetornarFalso()
        {
            var repositorio = new Mock<IUsuarioRepository>();
            repositorio.Setup(x => x.ObterUsuarioNaoExcluidoPorCpf(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException());
            var handler = new UsuarioDispositivoCommand.UsuarioDispositivoCommandHandler(repositorio.Object);

            var resultado = await handler.Handle(new UsuarioDispositivoCommand("123", "device"), CancellationToken.None);

            Assert.False(resultado);
        }
    }
}
