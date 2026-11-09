using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using SME.AE.Comum.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas.TermoDeUso
{
    public class ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryTeste
    {
        private readonly Mock<IAceiteTermosDeUsoRepositorio> repositorio;
        private readonly ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryHandler handler;

        public ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryTeste()
        {
            repositorio = new Mock<IAceiteTermosDeUsoRepositorio>();
            handler = new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryHandler(repositorio.Object);
        }

        [Fact]
        public void Construtor_RepositorioNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQueryHandler(null));

            Assert.Equal("aceiteTermosDeUsoRepositorio", excecao.ParamName);
        }

        [Fact]
        public async Task Handle_QuandoUsuarioAindaNaoAceitou_DeveRetornarFalso()
        {
            const string cpf = "12345678901";
            const double versao = 1.0;
            repositorio.Setup(x => x.ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(cpf, versao))
                .ReturnsAsync(false);

            var resultado = await handler.Handle(
                new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery(cpf, versao), CancellationToken.None);

            Assert.False(resultado);
            repositorio.Verify(x => x.ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(cpf, versao), Times.Once);
        }

        [Fact]
        public async Task Handle_QuandoUsuarioJaAceitou_DeveLancarNegocioException()
        {
            const string cpf = "12345678901";
            const double versao = 1.0;
            repositorio.Setup(x => x.ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(cpf, versao))
                .ReturnsAsync(true);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() => handler.Handle(
                new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery(cpf, versao), CancellationToken.None));

            Assert.Equal("Este usuário já aceitou os termos de uso!", excecao.Message);
        }

        [Fact]
        public async Task Handle_QuandoRepositorioFalhar_DevePropagarExcecao()
        {
            repositorio.Setup(x => x.ValidarAceiteDoTermoDeUsoPorUsuarioEVersao(It.IsAny<string>(), It.IsAny<double>()))
                .ThrowsAsync(new InvalidOperationException("falha ao validar aceite"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(
                new ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery("12345678901", 1.0), CancellationToken.None));

            Assert.Equal("falha ao validar aceite", excecao.Message);
        }
    }
}
