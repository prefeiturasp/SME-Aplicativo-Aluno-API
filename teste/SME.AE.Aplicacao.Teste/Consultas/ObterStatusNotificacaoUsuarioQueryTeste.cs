using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas
{
    public class ObterStatusNotificacaoUsuarioQueryTeste : BaseTeste
    {
        private const long CodigoAluno = 1234567;
        private const string CodigoAlunoTexto = "1234567";
        private const string CpfResponsavel = "12345678900";
        private const long IdNotificacao = 10;
        private const long OutraNotificacao = 20;
        private const long TerceiraNotificacao = 30;
        private const string SituacaoLida = "Lida - em 05/03/2026 14:30";
        private const string SituacaoNaoLida = "Não lida";
        private const string SituacaoNaoTemApp = "Não tem APP";
        private const string MensagemNotificacaoObrigatoria = "O ID da notificação é obrigatório";
        private const string MensagemCodigoAlunoObrigatorio = "O código da turma é obrigatório";

        private static readonly DateTime DataLeitura = new DateTime(2026, 3, 5, 14, 30, 45);

        private readonly ObterStatusNotificacaoUsuarioQueryHandler handler;
        private readonly ObterStatusNotificacaoUsuarioQueryValidator validator;
        private readonly Mock<IDadosLeituraRepositorio> dadosLeituraRepositorio;
        private readonly Mock<IUsuarioRepository> usuarioRepositorio;

        public enum CenarioSemUsuario
        {
            EolNulo,
            EolVazio,
            ResponsavelSemCadastro
        }

        public ObterStatusNotificacaoUsuarioQueryTeste()
        {
            dadosLeituraRepositorio = new Mock<IDadosLeituraRepositorio>();
            usuarioRepositorio = new Mock<IUsuarioRepository>();

            dadosLeituraRepositorio.Setup(r => r.ObterDadosLeituraAlunos(It.IsAny<long>(), It.IsAny<string>()))
                                   .ReturnsAsync(Enumerable.Empty<DataLeituraAluno>());

            handler = new ObterStatusNotificacaoUsuarioQueryHandler(dadosLeituraRepositorio.Object, usuarioRepositorio.Object, mediator.Object);
            validator = new ObterStatusNotificacaoUsuarioQueryValidator();
        }

        [Fact(DisplayName = "Deve consultar o aluno no EOL uma única vez, somente pelo código do aluno")]
        public async Task Deve_Consultar_O_Aluno_No_EOL_Uma_Unica_Vez_Somente_Pelo_Codigo_Do_Aluno()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();

            await Executar(IdNotificacao, OutraNotificacao);

            mediator.Verify(m => m.Send(It.Is<ObterDadosAlunosQuery>(q => q.CodigoAluno == CodigoAluno &&
                                                                        q.CpfResponsavel == null &&
                                                                        q.CodigoDre == null &&
                                                                        q.CodigoUe == null), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.IsAny<ObterDadosAlunosQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve buscar o usuário do app uma única vez pelo CPF do responsável do aluno")]
        public async Task Deve_Buscar_O_Usuario_Do_App_Uma_Unica_Vez_Pelo_CPF_Do_Responsavel_Do_Aluno()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();

            await Executar(IdNotificacao, OutraNotificacao);

            usuarioRepositorio.Verify(r => r.ObterPorCpf(CpfResponsavel), Times.Once);
            usuarioRepositorio.Verify(r => r.ObterPorCpf(It.IsAny<string>()), Times.Once);
        }

        [Fact(DisplayName = "Deve consultar a leitura de cada notificação pelo código do aluno")]
        public async Task Deve_Consultar_A_Leitura_De_Cada_Notificacao_Pelo_Codigo_Do_Aluno()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();

            await Executar(IdNotificacao, OutraNotificacao);

            dadosLeituraRepositorio.Verify(r => r.ObterDadosLeituraAlunos(IdNotificacao, CodigoAlunoTexto), Times.Once);
            dadosLeituraRepositorio.Verify(r => r.ObterDadosLeituraAlunos(OutraNotificacao, CodigoAlunoTexto), Times.Once);
            dadosLeituraRepositorio.Verify(r => r.ObterDadosLeituraAlunos(It.IsAny<long>(), It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact(DisplayName = "Deve retornar a notificação como lida com a data e a hora da leitura")]
        public async Task Deve_Retornar_A_Notificacao_Como_Lida_Com_A_Data_E_A_Hora_Da_Leitura()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();
            MockarLeitura(IdNotificacao);

            var resultado = await ExecutarComCulturaInvariante(IdNotificacao);

            ValidarStatus(Assert.Single(resultado), IdNotificacao, DataLeitura, SituacaoLida);
        }

        [Theory(DisplayName = "Deve retornar a notificação como lida mesmo sem usuário no app")]
        [InlineData(CenarioSemUsuario.ResponsavelSemCadastro)]
        [InlineData(CenarioSemUsuario.EolVazio)]
        [InlineData(CenarioSemUsuario.EolNulo)]
        public async Task Deve_Retornar_A_Notificacao_Como_Lida_Mesmo_Sem_Usuario_No_App(CenarioSemUsuario cenario)
        {
            MockarCenarioSemUsuario(cenario);
            MockarLeitura(IdNotificacao);

            var resultado = await ExecutarComCulturaInvariante(IdNotificacao);

            ValidarStatus(Assert.Single(resultado), IdNotificacao, DataLeitura, SituacaoLida);
        }

        [Fact(DisplayName = "Deve retornar a notificação como não lida quando o responsável tiver usuário e não houver leitura")]
        public async Task Deve_Retornar_A_Notificacao_Como_Nao_Lida_Quando_O_Responsavel_Tiver_Usuario_E_Nao_Houver_Leitura()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();

            var resultado = await Executar(IdNotificacao);

            ValidarStatus(Assert.Single(resultado), IdNotificacao, null, SituacaoNaoLida);
        }

        [Theory(DisplayName = "Deve retornar 'Não tem APP' quando não houver leitura e o responsável não tiver usuário")]
        [InlineData(CenarioSemUsuario.EolNulo, false)]
        [InlineData(CenarioSemUsuario.EolVazio, false)]
        [InlineData(CenarioSemUsuario.ResponsavelSemCadastro, true)]
        public async Task Deve_Retornar_Nao_Tem_App_Quando_Nao_Houver_Leitura_E_O_Responsavel_Nao_Tiver_Usuario(CenarioSemUsuario cenario, bool deveBuscarUsuario)
        {
            MockarCenarioSemUsuario(cenario);

            var resultado = await Executar(IdNotificacao);

            ValidarStatus(Assert.Single(resultado), IdNotificacao, null, SituacaoNaoTemApp);
            usuarioRepositorio.Verify(r => r.ObterPorCpf(It.IsAny<string>()), deveBuscarUsuario ? Times.Once() : Times.Never());
        }

        [Fact(DisplayName = "Deve retornar um status para cada notificação na ordem informada")]
        public async Task Deve_Retornar_Um_Status_Para_Cada_Notificacao_Na_Ordem_Informada()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();
            MockarLeitura(IdNotificacao);
            MockarLeitura(TerceiraNotificacao);

            var resultado = await ExecutarComCulturaInvariante(IdNotificacao, OutraNotificacao, TerceiraNotificacao);

            Assert.Collection(resultado,
                status => ValidarStatus(status, IdNotificacao, DataLeitura, SituacaoLida),
                status => ValidarStatus(status, OutraNotificacao, null, SituacaoNaoLida),
                status => ValidarStatus(status, TerceiraNotificacao, DataLeitura, SituacaoLida));
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando nenhuma notificação for informada")]
        public async Task Deve_Retornar_Lista_Vazia_Quando_Nenhuma_Notificacao_For_Informada()
        {
            MockarAlunosEol(CriarAluno());
            MockarUsuario();

            var resultado = await Executar();

            Assert.Empty(resultado);
            dadosLeituraRepositorio.Verify(r => r.ObterDadosLeituraAlunos(It.IsAny<long>(), It.IsAny<string>()), Times.Never);
        }

        [Theory(DisplayName = "Deve aceitar quando o código do aluno for informado e a lista de notificações existir")]
        [InlineData(true)]
        [InlineData(false)]
        public void Deve_Aceitar_Quando_O_Codigo_Do_Aluno_For_Informado_E_A_Lista_De_Notificacoes_Existir(bool comNotificacoes)
        {
            var query = comNotificacoes ? CriarQuery(IdNotificacao) : CriarQuery();

            var resultado = validator.TestValidate(query);

            resultado.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Deve exigir a lista de notificações")]
        public void Deve_Exigir_A_Lista_De_Notificacoes()
        {
            var resultado = validator.TestValidate(new ObterStatusNotificacaoUsuarioQuery(null, CodigoAluno));

            resultado.ShouldHaveValidationErrorFor(x => x.NotificoesId).WithErrorMessage(MensagemNotificacaoObrigatoria);
            resultado.ShouldNotHaveValidationErrorFor(x => x.CodigoAluno);
        }

        [Fact(DisplayName = "Deve exigir o código do aluno")]
        public void Deve_Exigir_O_Codigo_Do_Aluno()
        {
            var resultado = validator.TestValidate(new ObterStatusNotificacaoUsuarioQuery(new List<long> { IdNotificacao }, 0));

            resultado.ShouldHaveValidationErrorFor(x => x.CodigoAluno).WithErrorMessage(MensagemCodigoAlunoObrigatorio);
            resultado.ShouldNotHaveValidationErrorFor(x => x.NotificoesId);
        }

        private Task<IEnumerable<StatusNotificacaoUsuario>> Executar(params long[] notificacoesId)
        {
            return handler.Handle(CriarQuery(notificacoesId), new CancellationToken());
        }

        private async Task<IEnumerable<StatusNotificacaoUsuario>> ExecutarComCulturaInvariante(params long[] notificacoesId)
        {
            var culturaOriginal = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                return await Executar(notificacoesId);
            }
            finally
            {
                CultureInfo.CurrentCulture = culturaOriginal;
            }
        }

        private static ObterStatusNotificacaoUsuarioQuery CriarQuery(params long[] notificacoesId)
        {
            return new ObterStatusNotificacaoUsuarioQuery(notificacoesId.ToList(), CodigoAluno);
        }

        private static AlunoRespostaEol CriarAluno()
        {
            return new AlunoRespostaEol
            {
                CodigoEol = (int)CodigoAluno,
                CpfResponsavel = CpfResponsavel
            };
        }

        private void MockarAlunosEol(params AlunoRespostaEol[] alunos)
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterDadosAlunosQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(alunos);
        }

        private void MockarUsuario()
        {
            usuarioRepositorio.Setup(r => r.ObterPorCpf(CpfResponsavel))
                              .ReturnsAsync(new Usuario { Cpf = CpfResponsavel });
        }

        private void MockarLeitura(long notificacaoId)
        {
            dadosLeituraRepositorio.Setup(r => r.ObterDadosLeituraAlunos(notificacaoId, CodigoAlunoTexto))
                                   .ReturnsAsync(new[] { new DataLeituraAluno { CodigoAluno = CodigoAluno, DataLeitura = DataLeitura } });
        }

        private void MockarCenarioSemUsuario(CenarioSemUsuario cenario)
        {
            switch (cenario)
            {
                case CenarioSemUsuario.EolNulo:
                    MockarAlunosEol(null);
                    break;
                case CenarioSemUsuario.EolVazio:
                    MockarAlunosEol();
                    break;
                case CenarioSemUsuario.ResponsavelSemCadastro:
                    MockarAlunosEol(CriarAluno());
                    usuarioRepositorio.Setup(r => r.ObterPorCpf(CpfResponsavel)).ReturnsAsync((Usuario)null);
                    break;
            }
        }

        private static void ValidarStatus(StatusNotificacaoUsuario status, long notificacaoId, DateTime? dataHoraLeitura, string situacao)
        {
            Assert.Equal(notificacaoId, status.NotificacaoId);
            Assert.Equal(dataHoraLeitura, status.DataHoraLeitura);
            Assert.Equal(situacao, status.Situacao);
        }
    }
}
