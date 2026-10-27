using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Testes.Comandos.Autenticacao
{
    public class AutenticarUsuarioCommandHandlerTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<IUsuarioCoreSSORepositorio> _repositoryCoreSSOMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AutenticarUsuarioCommandHandler _handler;

        public AutenticarUsuarioCommandHandlerTests()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _repositoryCoreSSOMock = new Mock<IUsuarioCoreSSORepositorio>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AutenticarUsuarioCommandHandler(
                _repositoryMock.Object,
                _repositoryCoreSSOMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_PrimeiroAcesso_DataNascimentoInvalidaNaSenha_DeveRetornarFalha()
        {
            // Arrange
            var request = new AutenticarUsuarioCommand("12345678900", "00000000"); // Data inválida ex: 00/00/0000

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            _repositoryMock
                .Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync((Dominio.Entidades.Usuario)null);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Contains("Data de nascimento inválida.", resultado.Erros);
        }

        [Fact]
        public async Task Handle_PrimeiroAcesso_SemAlunosRelacionados_DeveDesativarEExcluirUsuario()
        {
            // Arrange
            var request = new AutenticarUsuarioCommand("12345678900", "20140524"); // Data válida: 24/05/2014

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            _repositoryMock
                .Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync((Dominio.Entidades.Usuario)null);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<DadosResponsavelAluno>()); // Nenhum aluno retornado

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Contains("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.", resultado.Erros);
        }

        [Fact]
        public async Task Handle_PrimeiroAcesso_DataNascimentoAlunoDivergente_DeveRetornarFalha()
        {
            // Arrange
            var request = new AutenticarUsuarioCommand("12345678900", "20140524"); // Data na senha: 24/05/2014

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            _repositoryMock
                .Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync((Dominio.Entidades.Usuario)null);

            var alunos = new List<DadosResponsavelAluno>
            {
                new DadosResponsavelAluno { DataNascimentoAluno = new DateTime(2010, 1, 1) } // Data diferente
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alunos);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Contains("Data de Nascimento inválida.", resultado.Erros);
        }

        [Theory]
        [InlineData("20140524")] // yyyyMMdd
        [InlineData("24052014")] // ddMMyyyy
        public async Task Handle_PrimeiroAcesso_FormatosDeDataValidos_DeveAutenticarComSucesso(string senhaData)
        {
            // Arrange
            var request = new AutenticarUsuarioCommand("12345678900", senhaData);
            var dataNascimentoEsperada = new DateTime(2014, 5, 24);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            _repositoryMock
                .Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync((Dominio.Entidades.Usuario)null);

            var alunos = new List<DadosResponsavelAluno>
            {
                new DadosResponsavelAluno
                {
                    Cpf = request.Cpf,
                    DataNascimentoAluno = dataNascimentoEsperada,
                    DataAtualizacao = DateTime.Now
                }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alunos);

            _repositoryCoreSSOMock
                .Setup(r => r.SelecionarGrupos())
                .ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            _repositoryMock
                .Setup(r => r.ObterUsuarioNaoExcluidoPorCpf(request.Cpf))
                .ReturnsAsync(new Dominio.Entidades.Usuario { Id = 1, Cpf = request.Cpf });

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(resultado.Ok);

            _repositoryMock.Verify(r => r.SalvarAsync(It.IsAny<Dominio.Entidades.Usuario>()), Times.Once);
        }
    }
}