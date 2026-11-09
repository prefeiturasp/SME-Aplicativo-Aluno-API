using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.TermoDeUso
{
    public class ValidarTermosDeUsoQueryTeste
    {
        private readonly Mock<ITermosDeUsoRepositorio> repositorio;
        private readonly ValidarTermosDeUsoQueryHandler handler;

        public ValidarTermosDeUsoQueryTeste()
        {
            repositorio = new Mock<ITermosDeUsoRepositorio>();
            handler = new ValidarTermosDeUsoQueryHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ValidarTermosDeUsoQueryHandler(null));

            Assert.Equal("termosDeUsoRepositorio", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_QuandoTermoExiste_DeveRetornarVerdadeiro()
        {
            const long termoId = 7;
            repositorio.Setup(x => x.ObterPorIdAsync(termoId))
                .ReturnsAsync(new TermosDeUso("termos", "politica", 1.0));

            var resultado = await handler.Handle(new ValidarTermosDeUsoQuery(termoId), CancellationToken.None);

            Assert.True(resultado);
            repositorio.Verify(x => x.ObterPorIdAsync(termoId), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoTermoNaoExiste_DeveLancarNegocioException()
        {
            const long termoId = 7;
            repositorio.Setup(x => x.ObterPorIdAsync(termoId)).ReturnsAsync((TermosDeUso)null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                handler.Handle(new ValidarTermosDeUsoQuery(termoId), CancellationToken.None));

            Assert.Equal("Não foi possível encontrar os Termos de Uso.", excecao.Message);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioFalhar_DevePropagarExcecao()
        {
            repositorio.Setup(x => x.ObterPorIdAsync(It.IsAny<long>()))
                .ThrowsAsync(new InvalidOperationException("falha ao validar termo"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new ValidarTermosDeUsoQuery(7), CancellationToken.None));

            Assert.Equal("falha ao validar termo", excecao.Message);
        }
    }
}
