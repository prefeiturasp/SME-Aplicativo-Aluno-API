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
    public class ObterTermosDeUsoPorCpfQueryTeste
    {
        private readonly Mock<ITermosDeUsoRepositorio> repositorio;
        private readonly ObterTermosDeUsoPorCpfQueryHandler handler;

        public ObterTermosDeUsoPorCpfQueryTeste()
        {
            repositorio = new Mock<ITermosDeUsoRepositorio>();
            handler = new ObterTermosDeUsoPorCpfQueryHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new ObterTermosDeUsoPorCpfQueryHandler(null));

            Assert.Equal("termosDeUsoRepositorio", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioRetornaTermo_DeveMapearParaDto()
        {
            const string cpf = "12345678901";
            var termo = new TermosDeUso("termos", "politica", 1.5) { Id = 7 };
            repositorio.Setup(x => x.ObterTermosDeUsoPorCpf(cpf)).ReturnsAsync(termo);

            var resultado = await handler.Handle(new ObterTermosDeUsoPorCpfQuery(cpf), CancellationToken.None);

            Assert.NotNull(resultado);
            Assert.Equal(termo.DescricaoTermosDeUso, resultado.TermosDeUso);
            Assert.Equal(termo.DescricaoPoliticaPrivacidade, resultado.PoliticaDePrivacidade);
            Assert.Equal(termo.Versao, resultado.Versao);
            Assert.Equal(termo.Id, resultado.Id);
            repositorio.Verify(x => x.ObterTermosDeUsoPorCpf(cpf), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioNaoRetornaTermo_DeveLancarNegocioException()
        {
            const string cpf = "12345678901";
            repositorio.Setup(x => x.ObterTermosDeUsoPorCpf(cpf)).ReturnsAsync((TermosDeUso)null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() =>
                handler.Handle(new ObterTermosDeUsoPorCpfQuery(cpf), CancellationToken.None));

            Assert.Equal("Nenhum termo de uso encontrado para o CPF informado", excecao.Message);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioFalhar_DevePropagarExcecao()
        {
            repositorio.Setup(x => x.ObterTermosDeUsoPorCpf(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("falha ao consultar termos"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                handler.Handle(new ObterTermosDeUsoPorCpfQuery("12345678901"), CancellationToken.None));

            Assert.Equal("falha ao consultar termos", excecao.Message);
        }
    }
}
