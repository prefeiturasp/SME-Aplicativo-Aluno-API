using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas
{
    public class ObterDadosAlunosQueryTeste : BaseTeste
    {
        private const string NomeClienteEol = "servicoApiEolChave";
        private const string UrlBaseEol = "http://eol.teste/api/";
        private const string RotaDadosAlunos = "alunos/dados-acompanhamento-escolar";
        private const string CpfResponsavel = "12345678900";
        private const long CodigoAluno = 1234567;
        private const int OutroCodigoAluno = 7654321;
        private const string CodigoDre = "108100";
        private const string CodigoUe = "019493";

        private readonly ObterDadosAlunosQueryHandler handler;
        private readonly Mock<IHttpClientFactory> httpClientFactory;
        private readonly Mock<HttpMessageHandler> httpMessageHandler;
        private HttpRequestMessage requisicaoEnviada;

        public ObterDadosAlunosQueryTeste()
        {
            httpMessageHandler = new Mock<HttpMessageHandler>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(NomeClienteEol))
                             .Returns(() => new HttpClient(httpMessageHandler.Object) { BaseAddress = new Uri(UrlBaseEol) });

            handler = new ObterDadosAlunosQueryHandler(httpClientFactory.Object);
        }

        [Fact(DisplayName = "Deve consultar os dados do aluno no EOL com os filtros informados")]
        public async Task Deve_Consultar_Os_Dados_Do_Aluno_No_EOL_Com_Os_Filtros_Informados()
        {
            MockarRespostaEol(CriarRespostaComAlunos(CriarAluno((int)CodigoAluno)));

            await handler.Handle(new ObterDadosAlunosQuery(CpfResponsavel, CodigoAluno, CodigoDre, CodigoUe), new CancellationToken());

            httpClientFactory.Verify(x => x.CreateClient(NomeClienteEol), Times.Once);
            Assert.Equal(HttpMethod.Get, requisicaoEnviada.Method);
            Assert.Equal($"{UrlBaseEol}{RotaDadosAlunos}?cpfResponsavel={CpfResponsavel}&codigoDre={CodigoDre}&codigoUe={CodigoUe}&codigoAluno={CodigoAluno}",
                         requisicaoEnviada.RequestUri.ToString());
        }

        [Fact(DisplayName = "Deve enviar vazios na URL os filtros não informados")]
        public async Task Deve_Enviar_Vazios_Na_URL_Os_Filtros_Nao_Informados()
        {
            MockarRespostaEol(CriarRespostaComAlunos(CriarAluno((int)CodigoAluno)));

            await handler.Handle(new ObterDadosAlunosQuery(null, CodigoAluno, null, null), new CancellationToken());

            Assert.Equal($"{UrlBaseEol}{RotaDadosAlunos}?cpfResponsavel=&codigoDre=&codigoUe=&codigoAluno={CodigoAluno}",
                         requisicaoEnviada.RequestUri.ToString());
        }

        [Fact(DisplayName = "Deve retornar os alunos retornados pelo EOL")]
        public async Task Deve_Retornar_Os_Alunos_Retornados_Pelo_EOL()
        {
            var alunosEol = new[] { CriarAluno((int)CodigoAluno), CriarAluno(OutroCodigoAluno) };
            MockarRespostaEol(CriarRespostaComAlunos(alunosEol));

            var resultado = await handler.Handle(new ObterDadosAlunosQuery(CpfResponsavel, null, null, null), new CancellationToken());

            Assert.Collection(resultado,
                aluno => ValidarAluno(alunosEol[0], aluno),
                aluno => ValidarAluno(alunosEol[1], aluno));
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando o EOL não encontrar alunos")]
        public async Task Deve_Retornar_Lista_Vazia_Quando_O_EOL_Nao_Encontrar_Alunos()
        {
            MockarRespostaEol(CriarRespostaComAlunos());

            var resultado = await handler.Handle(new ObterDadosAlunosQuery(null, CodigoAluno, null, null), new CancellationToken());

            Assert.NotNull(resultado);
            Assert.Empty(resultado);
        }

        [Fact(DisplayName = "Deve retornar nulo quando o EOL responder sem conteúdo")]
        public async Task Deve_Retornar_Nulo_Quando_O_EOL_Responder_Sem_Conteudo()
        {
            MockarRespostaEol(new HttpResponseMessage(HttpStatusCode.NoContent));

            var resultado = await handler.Handle(new ObterDadosAlunosQuery(null, CodigoAluno, null, null), new CancellationToken());

            Assert.Null(resultado);
        }

        [Theory(DisplayName = "Deve retornar nulo quando o EOL não responder com sucesso")]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task Deve_Retornar_Nulo_Quando_O_EOL_Nao_Responder_Com_Sucesso(HttpStatusCode statusCode)
        {
            var resposta = CriarRespostaComAlunos(CriarAluno((int)CodigoAluno));
            resposta.StatusCode = statusCode;
            MockarRespostaEol(resposta);

            var resultado = await handler.Handle(new ObterDadosAlunosQuery(null, CodigoAluno, null, null), new CancellationToken());

            Assert.Null(resultado);
        }

        private void MockarRespostaEol(HttpResponseMessage resposta)
        {
            httpMessageHandler.Protected()
                              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                              .Callback<HttpRequestMessage, CancellationToken>((requisicao, token) => requisicaoEnviada = requisicao)
                              .ReturnsAsync(resposta);
        }

        private static HttpResponseMessage CriarRespostaComAlunos(params AlunoRespostaEol[] alunos)
        {
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(alunos)) };
        }

        private static AlunoRespostaEol CriarAluno(int codigoEol)
        {
            return new AlunoRespostaEol
            {
                CodigoEol = codigoEol,
                Nome = $"Aluno {codigoEol}",
                CpfResponsavel = CpfResponsavel,
                CodigoEscola = CodigoUe,
                CodigoDre = CodigoDre
            };
        }

        private static void ValidarAluno(AlunoRespostaEol esperado, AlunoRespostaEol aluno)
        {
            Assert.Equal(esperado.CodigoEol, aluno.CodigoEol);
            Assert.Equal(esperado.Nome, aluno.Nome);
            Assert.Equal(esperado.CpfResponsavel, aluno.CpfResponsavel);
            Assert.Equal(esperado.CodigoEscola, aluno.CodigoEscola);
            Assert.Equal(esperado.CodigoDre, aluno.CodigoDre);
        }
    }
}
