using MediatR;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.TermosDeUso;
using SME.AE.Aplicacao.Comandos.TermosDeUso;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Consultas.TermosDeUso;
using SME.AE.Comum.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using EntidadeUsuario = SME.AE.Dominio.Entidades.Usuario;
using ObterUsuarioQuery = SME.AE.Aplicacao.Consultas.ObterUsuario.ObterUsuarioNaoExcluidoPorCpfQuery;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.TermoDeUso
{
    public class RegistrarAceiteDosTermosDeUsoUseCaseTeste
    {
        private readonly Mock<IMediator> mediator;
        private readonly RegistrarAceiteDosTermosDeUsoUseCase useCase;

        public RegistrarAceiteDosTermosDeUsoUseCaseTeste()
        {
            mediator = new Mock<IMediator>();
            useCase = new RegistrarAceiteDosTermosDeUsoUseCase(mediator.Object);
        }

        [Fact]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new RegistrarAceiteDosTermosDeUsoUseCase(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact]
        public async Task Executar_DadosValidos_DeveRegistrarAceiteERetornarTrue()
        {
            var aceite = CriarAceite();
            ConfigurarValidacoes();
            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntidadeUsuario { Cpf = aceite.CpfUsuario });
            mediator.Setup(x => x.Send(It.IsAny<RegistrarAceiteDosTermosDeUsoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var resultado = await useCase.Executar(aceite);

            Assert.True(resultado);
            VerificarEncaminhamento(aceite);
            mediator.Verify(x => x.Send(It.Is<RegistrarAceiteDosTermosDeUsoCommand>(command =>
                command.TermoDeUsoId == aceite.TermoDeUsoId &&
                command.Usuario == aceite.CpfUsuario &&
                command.Device == aceite.Device &&
                command.Ip == aceite.Ip &&
                command.Versao == aceite.Versao), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Executar_QuandoRegistroFalhar_DeveRetornarFalse()
        {
            var aceite = CriarAceite();
            ConfigurarValidacoes();
            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntidadeUsuario { Cpf = aceite.CpfUsuario });
            mediator.Setup(x => x.Send(It.IsAny<RegistrarAceiteDosTermosDeUsoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var resultado = await useCase.Executar(aceite);

            Assert.False(resultado);
        }

        [Fact]
        public async Task Executar_QuandoUsuarioNaoForLocalizado_DeveLancarNegocioException()
        {
            var aceite = CriarAceite();
            ConfigurarValidacoes();
            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((EntidadeUsuario)null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(() => useCase.Executar(aceite));

            Assert.Equal("Não localizamos um usuário com o CPF informado", excecao.Message);
            mediator.Verify(x => x.Send(It.IsAny<RegistrarAceiteDosTermosDeUsoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Executar_QuandoValidacaoFalhar_DevePropagarExcecao()
        {
            var aceite = CriarAceite();
            mediator.Setup(x => x.Send(It.IsAny<ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("falha na validação"));

            var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.Executar(aceite));

            Assert.Equal("falha na validação", excecao.Message);
            mediator.Verify(x => x.Send(It.IsAny<ValidarTermosDeUsoQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private void ConfigurarValidacoes()
        {
            mediator.Setup(x => x.Send(It.IsAny<ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            mediator.Setup(x => x.Send(It.IsAny<ValidarTermosDeUsoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }

        private void VerificarEncaminhamento(RegistrarAceiteDosTermosDeUsoDto aceite)
        {
            mediator.Verify(x => x.Send(It.Is<ValidarAceiteDoTermoDeUsoPorUsuarioEVersaoQuery>(query =>
                query.CpfUsuario == aceite.CpfUsuario && query.Versao == aceite.Versao), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.Is<ValidarTermosDeUsoQuery>(query =>
                query.TermoDeUsoId == aceite.TermoDeUsoId), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.Is<ObterUsuarioQuery>(query =>
                query.Cpf == aceite.CpfUsuario), It.IsAny<CancellationToken>()), Times.Once);
        }

        private static RegistrarAceiteDosTermosDeUsoDto CriarAceite()
        {
            return new RegistrarAceiteDosTermosDeUsoDto
            {
                TermoDeUsoId = 1,
                CpfUsuario = "12345678901",
                Device = "Android 14",
                Ip = "192.168.0.1",
                Versao = 1.0
            };
        }
    }
}
