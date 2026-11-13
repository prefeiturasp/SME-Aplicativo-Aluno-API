using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Autenticacao.CriarUsuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Testes.Comandos.Autenticacao
{
    public class AutenticarUsuarioCommandHandlerTests
    {
        private const string CpfValido = "12345678909";

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
            var request = new AutenticarUsuarioCommand(CpfValido, "00000000");

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
            var request = new AutenticarUsuarioCommand(CpfValido, "20140524");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            _repositoryMock
                .Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync((Dominio.Entidades.Usuario)null);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var resultado = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Contains("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.", resultado.Erros);
        }

        [Fact]
        public async Task Handle_PrimeiroAcesso_DataNascimentoAlunoDivergente_DeveRetornarFalha()
        {
            // Arrange
            var request = new AutenticarUsuarioCommand(CpfValido, "20140524");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            _repositoryMock
                .Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync((Dominio.Entidades.Usuario)null);

            var alunos = new List<DadosResponsavelAluno>
            {
                new DadosResponsavelAluno { DataNascimentoAluno = new DateTime(2010, 1, 1, 0, 0, 0,DateTimeKind.Unspecified) }
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
        [InlineData("20140524")]
        [InlineData("24052014")]
        public async Task Handle_PrimeiroAcesso_FormatosDeDataValidos_DeveAutenticarComSucesso(string senhaData)
        {
            // Arrange
            var request = new AutenticarUsuarioCommand(CpfValido, senhaData);
            var dataNascimentoEsperada = new DateTime(2014, 5, 24, 0, 0, 0, DateTimeKind.Unspecified);

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

        [Fact]
        public async Task Handle_SolicitacaoInvalida_DeveRetornarFalhaDeValidacao()
        {
            var request = new AutenticarUsuarioCommand("11111111111", string.Empty);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.False(resultado.Ok);
            Assert.NotEmpty(resultado.Erros);
            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UsuarioExistente_SenhaIncorreta_DeveRetornarFalha()
        {
            var request = new AutenticarUsuarioCommand(CpfValido, "senha-incorreta");
            var usuarioCoreSSO = new RetornoUsuarioCoreSSO
            {
                Senha = GerarHashSha512("senha-correta"),
                TipoCriptografia = TipoCriptografia.SHA512,
                Grupos = [Guid.NewGuid()]
            };

            ConfigurarUsuarioCoreSSO(usuarioCoreSSO);
            _repositoryMock.Setup(r => r.ObterPorCpf(request.Cpf))
                .ReturnsAsync(new Dominio.Entidades.Usuario { Id = 1, Cpf = request.Cpf, PrimeiroAcesso = false });

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Contains("Usuário ou senha incorretos.", resultado.Erros);
        }

        [Fact]
        public async Task Handle_PrimeiroAcesso_AlunoComRestricao_DeveRetornarFalha()
        {
            var request = new AutenticarUsuarioCommand(CpfValido, "20140524");
            ConfigurarUsuarioCoreSSO(null);
            _repositoryMock.Setup(r => r.ObterPorCpf(request.Cpf)).ReturnsAsync((Dominio.Entidades.Usuario)null);
            ConfigurarAlunos(new DadosResponsavelAluno
            {
                DataNascimentoAluno = new DateTime(2014, 5, 24, 0, 0, 0, DateTimeKind.Unspecified),
                TipoSigilo = (int)AlunoTipoSigilo.Restricao
            });

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Contains("Usuário não cadastrado, qualquer dúvida procure a unidade escolar.", resultado.Erros);
        }

        [Fact]
        public async Task Handle_UsuarioExistenteSemAlunos_DeveExcluirEInativarUsuario()
        {
            var request = new AutenticarUsuarioCommand(CpfValido, "senha-correta");
            var usuarioCoreSSO = new RetornoUsuarioCoreSSO
            {
                UsuId = Guid.NewGuid(),
                Senha = GerarHashSha512(request.Senha),
                TipoCriptografia = TipoCriptografia.SHA512,
                Grupos = new[] { Guid.NewGuid() }
            };
            var usuarioApp = new Dominio.Entidades.Usuario { Id = 1, Cpf = request.Cpf, PrimeiroAcesso = false };

            ConfigurarUsuarioCoreSSO(usuarioCoreSSO);
            _repositoryMock.Setup(r => r.ObterPorCpf(request.Cpf)).ReturnsAsync(usuarioApp);
            ConfigurarAlunos();

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.Contains("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.", resultado.Erros);
            _repositoryMock.Verify(r => r.ExcluirUsuario(request.Cpf), Times.Once);
            _repositoryCoreSSOMock.Verify(r => r.AlterarStatusUsuario(usuarioCoreSSO.UsuId, StatusUsuarioCoreSSO.Inativo), Times.Once);
        }

        [Fact]
        public async Task Handle_UsuarioSemGrupos_DeveAtualizarAcessoEAutenticar()
        {
            var request = new AutenticarUsuarioCommand(CpfValido, "senha-correta");
            var usuarioCoreSSO = new RetornoUsuarioCoreSSO
            {
                Senha = GerarHashSha512(request.Senha),
                TipoCriptografia = TipoCriptografia.SHA512,
                Grupos = new[] { Guid.NewGuid() }
            };
            var usuario = ConfigurarUsuarioParaSucesso(request, usuarioCoreSSO, Array.Empty<Guid>(), false);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.True(resultado.Ok);
            Assert.True(Assert.IsType<RespostaAutenticar>(resultado.Data).PrimeiroAcesso);
            _repositoryCoreSSOMock.Verify(r => r.AtualizarCriptografiaUsuario(usuarioCoreSSO.UsuId, It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.AltualizarUltimoAcessoPrimeiroUsuario(usuario), Times.Once);
        }

        [Fact]
        public async Task Handle_UsuarioComGruposNulosNoCoreSSO_DeveAutenticarComoPrimeiroAcesso()
        {
            var request = new AutenticarUsuarioCommand(CpfValido, "senha-correta");
            var usuarioCoreSSO = new RetornoUsuarioCoreSSO
            {
                Senha = GerarHashSha512(request.Senha),
                TipoCriptografia = TipoCriptografia.SHA512,
                Grupos = null
            };
            var usuario = ConfigurarUsuarioParaSucesso(request, usuarioCoreSSO, new[] { Guid.NewGuid() }, false);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.True(resultado.Ok);
            Assert.True(Assert.IsType<RespostaAutenticar>(resultado.Data).PrimeiroAcesso);
            _repositoryMock.Verify(r => r.AltualizarUltimoAcessoPrimeiroUsuario(usuario), Times.Once);
        }

        [Fact]
        public async Task Handle_UsuarioComGrupoCorrespondente_UsuarioInativoECriptografiaAntiga_DeveAtualizarCoreSSO()
        {
            var request = new AutenticarUsuarioCommand(CpfValido, "senha-correta");
            var grupo = Guid.NewGuid();
            var usuarioCoreSSO = new RetornoUsuarioCoreSSO
            {
                UsuId = Guid.NewGuid(),
                Senha = GerarHashSha512(request.Senha),
                TipoCriptografia = TipoCriptografia.SHA512,
                Status = (int)StatusUsuarioCoreSSO.Inativo,
                Grupos = [grupo]
            };
            var usuario = ConfigurarUsuarioParaSucesso(request, usuarioCoreSSO, new[] { grupo }, false);

            var resultado = await _handler.Handle(request, CancellationToken.None);

            Assert.True(resultado.Ok);
            Assert.False(Assert.IsType<RespostaAutenticar>(resultado.Data).PrimeiroAcesso);
            _repositoryCoreSSOMock.Verify(r => r.AlterarStatusUsuario(usuarioCoreSSO.UsuId, StatusUsuarioCoreSSO.Ativo), Times.Once);
            _repositoryCoreSSOMock.Verify(r => r.AtualizarCriptografiaUsuario(usuarioCoreSSO.UsuId, It.IsAny<string>()), Times.Once);
            _repositoryMock.Verify(r => r.AltualizarUltimoAcessoPrimeiroUsuario(usuario), Times.Once);
        }

        private void ConfigurarUsuarioCoreSSO(RetornoUsuarioCoreSSO usuarioCoreSSO)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(usuarioCoreSSO);
        }

        private void ConfigurarAlunos(params DadosResponsavelAluno[] alunos)
        {
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alunos);
        }

        private Dominio.Entidades.Usuario ConfigurarUsuarioParaSucesso(
            AutenticarUsuarioCommand request,
            RetornoUsuarioCoreSSO usuarioCoreSSO,
            IEnumerable<Guid> grupos,
            bool primeiroAcesso)
        {
            var usuario = new Dominio.Entidades.Usuario
            {
                Id = 1,
                Cpf = request.Cpf,
                PrimeiroAcesso = primeiroAcesso
            };
            ConfigurarUsuarioCoreSSO(usuarioCoreSSO);
            _repositoryMock.Setup(r => r.ObterPorCpf(request.Cpf)).ReturnsAsync(usuario);
            ConfigurarAlunos(new DadosResponsavelAluno
            {
                Cpf = request.Cpf,
                DataNascimento = new DateTime(1990, 1, 1, 0, 0, 0,DateTimeKind.Unspecified),
                DataNascimentoAluno = new DateTime(2014, 5, 24, 0, 0, 0, DateTimeKind.Unspecified),
                DataAtualizacao = DateTime.Now,
                Nome = "Responsável",
                NomeMae = "Mãe",
                Email = "responsavel@teste.com",
                NumeroCelular = "999999999",
                DDDCelular = "11"
            });
            _repositoryCoreSSOMock.Setup(r => r.SelecionarGrupos()).ReturnsAsync([.. grupos]);
            _repositoryMock.Setup(r => r.ObterUsuarioNaoExcluidoPorCpf(request.Cpf)).ReturnsAsync(usuario);
            return usuario;
        }

        private static string GerarHashSha512(string senha)
        {
            return Convert.ToBase64String(SHA512.HashData(Encoding.Unicode.GetBytes(senha))).TrimStart('/');
        }
    }
}