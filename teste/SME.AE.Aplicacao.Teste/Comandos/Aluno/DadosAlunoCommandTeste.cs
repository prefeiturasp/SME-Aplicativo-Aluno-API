using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Aluno;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos.Aluno
{
    public class DadosAlunoCommandTeste
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DadosAlunoCommand.DadosAlunoComandoHandler _handler;

        public DadosAlunoCommandTeste()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new DadosAlunoCommand.DadosAlunoComandoHandler(_mediatorMock.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando o mediator for nulo")]
        public void Construtor_MediatorNulo_DeveLancarArgumentNullException()
        {
            // Act & Assert
            var excecao = Assert.Throws<ArgumentNullException>(() =>
                new DadosAlunoCommand.DadosAlunoComandoHandler(null));

            Assert.Equal("mediator", excecao.ParamName);
        }

        [Fact(DisplayName = "Deve retornar RespostaApi de falha quando não encontrar alunos para o CPF")]
        public async Task Handle_SemAlunosAtivos_DeveRetornarFalhaComMensagemDeNegocio()
        {
            // Arrange
            var cpf = "12345678901";
            var command = new DadosAlunoCommand(cpf);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosAlunosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Ok);
            Assert.Contains("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.", resultado.Erros);

            _mediatorMock.Verify(m => m.Send(It.IsAny<ObterTurmasModalidadesPorCodigosQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar RespostaApi de falha quando a consulta de alunos retornar nulo")]
        public async Task Handle_ConsultaAlunosRetornaNulo_DeveRetornarFalha()
        {
            // Arrange
            var command = new DadosAlunoCommand("12345678901");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosAlunosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<AlunoRespostaEol>)null);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Ok);
            Assert.Contains("Este CPF não está relacionado como responsável de um aluno ativo na rede municipal.", resultado.Erros);
        }

        [Fact(DisplayName = "Deve retornar RespostaApi de falha quando não encontrar modalidades das turmas no SGP")]
        public async Task Handle_SemModalidadesNoSgp_DeveRetornarFalhaComMensagemDeNegocio()
        {
            // Arrange
            var cpf = "12345678901";
            var command = new DadosAlunoCommand(cpf);
            var alunosEol = CriarListaAlunosPadrao(cpf);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosAlunosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alunosEol);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterTurmasModalidadesPorCodigosQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<TurmaModalidadeCodigoDto>());

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Ok);
            Assert.Contains("Não foi possível obter a(s) modalidade(s) da(s) turma(s) no SGP ", resultado.Erros);
        }

        [Fact(DisplayName = "Deve agrupar alunos por modalidade e aplicar regra de NomeSocial corretamente com sucesso")]
        public async Task Handle_DadosValidos_DeveRetornarSucessoComAlunosAgrupadosPorModalidade()
        {
            // Arrange
            var cpf = "12345678901";
            var command = new DadosAlunoCommand(cpf);
            var alunosEol = CriarListaAlunosPadrao(cpf);

            var modalidades = new List<TurmaModalidadeCodigoDto>
            {
                new TurmaModalidadeCodigoDto
                {
                    TurmaCodigo = 1001,
                    ModalidadeCodigo = 5,
                    ModalidadeDescricao = "Ensino Fundamental"
                },
                new TurmaModalidadeCodigoDto
                {
                    TurmaCodigo = 2002,
                    ModalidadeCodigo = 6,
                    ModalidadeDescricao = "Ensino Médio"
                }
            };

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterDadosAlunosQuery>(q => q.CpfResponsavel == cpf),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(alunosEol);

            _mediatorMock
                .Setup(m => m.Send(
                    It.Is<ObterTurmasModalidadesPorCodigosQuery>(q =>
                        q.TurmaCodigo.Contains("1001") &&
                        q.TurmaCodigo.Contains("2002") &&
                        q.TurmaCodigo.Length == 2),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(modalidades);

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.True(resultado.Ok);
            Assert.Empty(resultado.Erros);
            Assert.NotNull(resultado.Data);

            var listaEscolas = Assert.IsType<IEnumerable<ListaEscola>>(resultado.Data, exactMatch: false).ToList();
            Assert.Equal(2, listaEscolas.Count);

            var grupoFundamental = listaEscolas.First(x => x.ModalidadeCodigo == 5);
            Assert.Equal("Ensino Fundamental", grupoFundamental.Modalidade);
            var alunoFundamental = Assert.Single(grupoFundamental.Alunos);
            Assert.Equal("Responsavel Padrao", alunoFundamental.NomeResponsavel);
            Assert.Equal(new DateTime(2014, 5, 10, 0, 0, 0, DateTimeKind.Unspecified), alunoFundamental.DataNascimento);

            var grupoMedio = listaEscolas.First(x => x.ModalidadeCodigo == 6);
            Assert.Equal("Ensino Médio", grupoMedio.Modalidade);
            var alunoMedio = Assert.Single(grupoMedio.Alunos);
            Assert.Equal("Nome Social Aluno", alunoMedio.NomeResponsavel);
        }

        [Fact(DisplayName = "Deve capturar exceção inesperada e retornar RespostaApi de falha")]
        public async Task Handle_QuandoOcorreExcecaoInesperada_DeveRetornarRespostaApiFalha()
        {
            // Arrange
            var command = new DadosAlunoCommand("12345678901");

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<ObterDadosAlunosQuery>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Falha de conexão com EOL"));

            // Act
            var resultado = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(resultado);
            Assert.False(resultado.Ok);
            Assert.Contains("Falha de conexão com EOL", resultado.Erros);
        }

        private static List<AlunoRespostaEol> CriarListaAlunosPadrao(string cpf)
        {
            return new List<AlunoRespostaEol>
            {
                new AlunoRespostaEol
                {
                    CodigoEol = 12345,
                    Nome = "Aluno Um",
                    NomeResponsavel = "  Responsavel Padrao  ",
                    NomeSocial = null,
                    TipoResponsavel = TipoResponsavelEnum.Mae,
                    CpfResponsavel = cpf,
                    DataNascimento = new DateTime(2014, 5, 10, 14, 30, 0, DateTimeKind.Unspecified),
                    CodigoTipoEscola = 1,
                    CodigoEscola = "019283",
                    DescricaoTipoEscola = "EMEF",
                    Escola = "Escola Teste 1",
                    CodigoDre = "108100",
                    SiglaDre = "DRE-BT",
                    CodigoTurma = 1001,
                    Turma = "5A",
                    SituacaoMatricula = "Ativo",
                    DataSituacaoMatricula = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    SerieResumida = "5"
                },
                new AlunoRespostaEol
                {
                    CodigoEol = 67890,
                    Nome = "Aluno Dois",
                    NomeResponsavel = "  Nome Civil  ",
                    NomeSocial = "  Nome Social Aluno  ",
                    TipoResponsavel = TipoResponsavelEnum.Proprio_Aluno,
                    CpfResponsavel = cpf,
                    DataNascimento = new DateTime(2008, 8, 20, 8, 0, 0,DateTimeKind.Unspecified),
                    CodigoTipoEscola = 2,
                    CodigoEscola = "019284",
                    DescricaoTipoEscola = "EMEFM",
                    Escola = "Escola Teste 2",
                    CodigoDre = "108100",
                    SiglaDre = "DRE-BT",
                    CodigoTurma = 2002,
                    Turma = "1B",
                    SituacaoMatricula = "Ativo",
                    DataSituacaoMatricula = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Unspecified),
                    SerieResumida = "1"
                }
            };
        }
    }
}