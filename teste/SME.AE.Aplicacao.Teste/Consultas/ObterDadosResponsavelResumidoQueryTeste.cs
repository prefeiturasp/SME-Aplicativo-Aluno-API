using FluentValidation.TestHelper;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Consultas
{
    public class ObterDadosResponsavelResumidoQueryTeste : BaseTeste
    {
        private const string CpfResponsavel = "12345678900";
        private const string NomeClienteEol = "servicoApiEolChave";
        private const string UrlBaseEol = "http://eol.teste/api/";
        private const string MensagemCpfObrigatorio = "O CPF é Obrigátorio";

        private readonly ObterDadosResponsavelResumidoQueryHandler handler;
        private readonly ObterDadosResponsavelResumidoQueryValidator validator;
        private readonly Mock<IHttpClientFactory> httpClientFactory;
        private readonly Mock<HttpMessageHandler> httpMessageHandler;
        private HttpRequestMessage requisicaoEnviada;

        public ObterDadosResponsavelResumidoQueryTeste()
        {
            httpMessageHandler = new Mock<HttpMessageHandler>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(NomeClienteEol))
                             .Returns(() => new HttpClient(httpMessageHandler.Object) { BaseAddress = new Uri(UrlBaseEol) });

            handler = new ObterDadosResponsavelResumidoQueryHandler(httpClientFactory.Object);
            validator = new ObterDadosResponsavelResumidoQueryValidator();
        }

        [Fact(DisplayName = "Deve consultar o responsável resumido no EOL pelo CPF")]
        public async Task Deve_Consultar_O_Responsavel_Resumido_No_EOL_Pelo_CPF()
        {
            MockarRespostaEol(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(CriarResponsavel())) });

            await handler.Handle(new ObterDadosResponsavelResumidoQuery(CpfResponsavel), new CancellationToken());

            httpClientFactory.Verify(x => x.CreateClient(NomeClienteEol), Times.Once);
            Assert.Equal(HttpMethod.Get, requisicaoEnviada.Method);
            Assert.Equal($"{UrlBaseEol}alunos/responsaveis/{CpfResponsavel}/resumido", requisicaoEnviada.RequestUri.ToString());
        }

        [Fact(DisplayName = "Deve retornar os dados do responsável retornados pelo EOL")]
        public async Task Deve_Retornar_Os_Dados_Do_Responsavel_Retornados_Pelo_EOL()
        {
            var responsavelEol = CriarResponsavel();
            MockarRespostaEol(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(JsonConvert.SerializeObject(responsavelEol)) });

            var resultado = await handler.Handle(new ObterDadosResponsavelResumidoQuery(CpfResponsavel), new CancellationToken());

            Assert.NotNull(resultado);
            Assert.Equal(responsavelEol.Cpf, resultado.Cpf);
            Assert.Equal(responsavelEol.Nome, resultado.Nome);
            Assert.Equal(responsavelEol.Email, resultado.Email);
            Assert.Equal(responsavelEol.DDDCelular, resultado.DDDCelular);
            Assert.Equal(responsavelEol.NumeroCelular, resultado.NumeroCelular);
            Assert.Equal(responsavelEol.NomeMae, resultado.NomeMae);
        }

        [Fact(DisplayName = "Deve retornar nulo quando o EOL responder sem conteúdo")]
        public async Task Deve_Retornar_Nulo_Quando_O_EOL_Responder_Sem_Conteudo()
        {
            MockarRespostaEol(new HttpResponseMessage(HttpStatusCode.NoContent));

            var resultado = await handler.Handle(new ObterDadosResponsavelResumidoQuery(CpfResponsavel), new CancellationToken());

            Assert.Null(resultado);
        }

        [Theory(DisplayName = "Deve exigir o CPF do responsável")]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deve_Exigir_O_CPF_Do_Responsavel(string cpf)
        {
            var resultado = validator.TestValidate(new ObterDadosResponsavelResumidoQuery(cpf));

            resultado.ShouldHaveValidationErrorFor(x => x.CpfResponsavel).WithErrorMessage(MensagemCpfObrigatorio);
        }

        [Fact(DisplayName = "Deve aceitar quando o CPF do responsável for informado")]
        public void Deve_Aceitar_Quando_O_CPF_Do_Responsavel_For_Informado()
        {
            var resultado = validator.TestValidate(new ObterDadosResponsavelResumidoQuery(CpfResponsavel));

            resultado.ShouldNotHaveAnyValidationErrors();
        }

        private void MockarRespostaEol(HttpResponseMessage resposta)
        {
            httpMessageHandler.Protected()
                              .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                              .Callback<HttpRequestMessage, CancellationToken>((requisicao, token) => requisicaoEnviada = requisicao)
                              .ReturnsAsync(resposta);
        }

        private static DadosResponsavelAlunoResumido CriarResponsavel()
        {
            return new DadosResponsavelAlunoResumido
            {
                Cpf = CpfResponsavel,
                Nome = "Maria da Silva",
                Email = "responsavel@teste.com",
                DDDCelular = "11",
                NumeroCelular = "987654321",
                NomeMae = "Ana da Silva"
            };
        }
    }
}
