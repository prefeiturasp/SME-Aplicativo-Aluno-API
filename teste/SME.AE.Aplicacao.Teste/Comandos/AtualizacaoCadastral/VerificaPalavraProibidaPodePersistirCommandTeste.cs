using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using SME.AE.Aplicacao;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class VerificaPalavraProibidaPodePersistirCommandTeste
    {
        [Fact]
        public void Construtor_ComDependenciaNula_DeveLancarExcecao()
        {
            var mediator = new Mock<IMediator>();
            var cache = new Mock<ICacheRepositorio>();

            Assert.Throws<ArgumentNullException>(() => new VerificaPalavraProibidaPodePersistirCommandHandler(null, cache.Object));
            Assert.Throws<ArgumentNullException>(() => new VerificaPalavraProibidaPodePersistirCommandHandler(mediator.Object, null));
        }

        [Fact]
        public async Task Handle_ComCacheSemPalavras_DevePermitirPersistenciaENaoSalvarCache()
        {
            var mediator = new Mock<IMediator>();
            var cache = new Mock<ICacheRepositorio>();
            cache.Setup(x => x.Obter("palavras-bloqueadas", false)).Returns((string)null);
            mediator.Setup(x => x.Send(It.IsAny<ObterPalavrasProibidasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<string>());
            var handler = new VerificaPalavraProibidaPodePersistirCommandHandler(mediator.Object, cache.Object);

            var resultado = await handler.Handle(new VerificaPalavraProibidaPodePersistirCommand("qualquer texto"), CancellationToken.None);

            Assert.True(resultado);
            cache.Verify(x => x.SalvarAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComConsultaNula_DevePermitirPersistencia()
        {
            var mediator = new Mock<IMediator>();
            var cache = new Mock<ICacheRepositorio>();
            cache.Setup(x => x.Obter("palavras-bloqueadas", false)).Returns((string)null);
            mediator.Setup(x => x.Send(It.IsAny<ObterPalavrasProibidasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((string[])null);
            var handler = new VerificaPalavraProibidaPodePersistirCommandHandler(mediator.Object, cache.Object);

            var resultado = await handler.Handle(new VerificaPalavraProibidaPodePersistirCommand("texto"), CancellationToken.None);

            Assert.True(resultado);
        }

        [Fact]
        public async Task Handle_ComPalavrasConsultadas_DeveSalvarCacheEBloquearPalavraEncontrada()
        {
            var mediator = new Mock<IMediator>();
            var cache = new Mock<ICacheRepositorio>();
            cache.Setup(x => x.Obter("palavras-bloqueadas", false)).Returns((string)null);
            mediator.Setup(x => x.Send(It.IsAny<ObterPalavrasProibidasQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(["proibida", ""]);
            var handler = new VerificaPalavraProibidaPodePersistirCommandHandler(mediator.Object, cache.Object);

            var resultado = await handler.Handle(new VerificaPalavraProibidaPodePersistirCommand("Texto com PALAVRAPROIBIDA no meio"), CancellationToken.None);

            Assert.False(resultado);
            cache.Verify(x => x.SalvarAsync(
                "palavras-bloqueadas",
                It.Is<object>(valor => valor.GetType() == typeof(string[]) &&
                    ((string[])valor).Length == 2 &&
                    ((string[])valor)[0] == "proibida" &&
                    ((string[])valor)[1] == "")), Times.Once);
        }

        [Fact]
        public async Task Handle_ComCachePreenchidoESemCorrespondencia_DevePermitirPersistenciaSemConsultarMediator()
        {
            var mediator = new Mock<IMediator>();
            var cache = new Mock<ICacheRepositorio>();
            cache.Setup(x => x.Obter("palavras-bloqueadas", false)).Returns("[\"proibida\",null]");
            var handler = new VerificaPalavraProibidaPodePersistirCommandHandler(mediator.Object, cache.Object);

            var resultado = await handler.Handle(new VerificaPalavraProibidaPodePersistirCommand("texto permitido"), CancellationToken.None);

            Assert.True(resultado);
            mediator.Verify(x => x.Send(It.IsAny<ObterPalavrasProibidasQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ComCachePreenchidoECorrespondencia_DeveBloquearPersistencia()
        {
            var mediator = new Mock<IMediator>();
            var cache = new Mock<ICacheRepositorio>();
            cache.Setup(x => x.Obter("palavras-bloqueadas", false)).Returns("[\"proibida\"]");
            var handler = new VerificaPalavraProibidaPodePersistirCommandHandler(mediator.Object, cache.Object);

            var resultado = await handler.Handle(new VerificaPalavraProibidaPodePersistirCommand("uma proibida palavra"), CancellationToken.None);

            Assert.False(resultado);
        }
    }
}
