using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.TermoDeUso
{
    public class ObterTermosDeUsoQueryTeste
    {
        private readonly Mock<ITermosDeUsoRepositorio> repositorio;
        private readonly ObterTermosDeUsoQueryHandler handler;

        public ObterTermosDeUsoQueryTeste()
        {
            repositorio = new Mock<ITermosDeUsoRepositorio>();
            handler = new ObterTermosDeUsoQueryHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterTermosDeUsoQueryHandler(null));

            Assert.Equal("termosDeUsoRepositorio", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioRetornaTermo_DeveMapearParaDto()
        {
            var termo = new TermosDeUso("termos", "politica", 1.5) { Id = 7 };
            repositorio.Setup(x => x.ObterUltimaVersaoTermosDeUso()).ReturnsAsync(termo);

            var resultado = await handler.Handle(new ObterTermosDeUsoQuery(), CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(termo.DescricaoTermosDeUso, resultado.TermosDeUso);
            Assert.Equal(termo.DescricaoPoliticaPrivacidade, resultado.PoliticaDePrivacidade);
            Assert.Equal(termo.Versao, resultado.Versao);
            Assert.Equal(termo.Id, resultado.Id);
            repositorio.Verify(x => x.ObterUltimaVersaoTermosDeUso(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioNaoRetornaTermo_DeveRetornarNulo()
        {
            repositorio.Setup(x => x.ObterUltimaVersaoTermosDeUso()).ReturnsAsync((TermosDeUso)null);

            var resultado = await handler.Handle(new ObterTermosDeUsoQuery(), CancellationToken.None);

            Assert.Null(resultado);
            repositorio.Verify(x => x.ObterUltimaVersaoTermosDeUso(), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioFalhar_DevePropagarExcecao()
        {
            repositorio.Setup(x => x.ObterUltimaVersaoTermosDeUso())
                .ThrowsAsync(new InvalidOperationException("falha ao consultar termos"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new ObterTermosDeUsoQuery(), CancellationToken.None));

            Assert.Equal("falha ao consultar termos", excecao.Message);
        }
    }
}
