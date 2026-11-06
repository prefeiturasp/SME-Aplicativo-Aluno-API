using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Aplicacao.Consultas.VerificarSenha;
using SME.AE.Comum.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Usuario
{
    public class AlterarSenhaUseCaseTeste : BaseTeste
    {
        private readonly IAlterarSenhaUseCase alterarSenhaUseCase;
        private const string TokenMock = "token-jwt-valido-mock";

        public AlterarSenhaUseCaseTeste()
        {
            alterarSenhaUseCase = new AlterarSenhaUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Deve lançar ArgumentNullException quando o mediator for nulo")]
        public void Deve_Lancar_ArgumentNullException_Quando_Mediator_Nulo()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new AlterarSenhaUseCase(null));
            Assert.Equal("mediator", excecao.ParamName);
        }

        [Theory(DisplayName = "Deve Alterar a Senha")]
        [InlineData("23891476159", "Ab#12345", "Ab#12345")]
        [InlineData("09900791126", "Sgp@1234", "Ab#12345")]
        [InlineData("61056974141", "Aa!11111", "Ab#12345")]
        [InlineData("77083696144", "#Aa12345", "Ab#12345")]
        [InlineData("49072009193", "Ab#12345", "Ab#12345")]
        [InlineData("95461504108", "Sgp@1234", "Ab#12345")]
        [InlineData("91063385180", "Aa!11111", "Ab#12345")]
        [InlineData("94377608100", "#Aa12345", "Ab#12345")]
        public async Task Deve_Alterar_Senha(string cpf, string senha, string senhaAnterior)
        {
            InstanciarSetup();

            var dto = new AlterarSenhaDto(cpf, senha);

            var resposta = await alterarSenhaUseCase.Executar(dto, senhaAnterior);

            Assert.NotNull(resposta);
            Assert.True(resposta.Ok);
            Assert.Empty(resposta.Erros);

            var retornoToken = Assert.IsType<RetornoToken>(resposta.Data);
            Assert.NotNull(retornoToken);

            mediator.Verify(x => x.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<VerificarUltimasSenhasQuery>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<AdicionarSenhaHistoricoCommand>(), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(x => x.Send(It.IsAny<CriarTokenCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar NegocioException quando usuário não for encontrado no CoreSSO")]
        public async Task Deve_Lancar_NegocioException_Quando_Usuario_Nao_Encontrado()
        {
            InstanciarSetup();
            var cpf = "23891476159";
            var dto = new AlterarSenhaDto(cpf, "Sgp@1234");

            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((RetornoUsuarioCoreSSO)null);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                async () => await alterarSenhaUseCase.Executar(dto, "Ab#12345"));

            Assert.Equal($"Usuário com o CPF '{cpf}' não encontrado", excecao.Message);
            mediator.Verify(x => x.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact(DisplayName = "Deve lançar NegocioException quando a nova senha estiver entre as últimas 5 utilizadas")]
        public async Task Deve_Lancar_NegocioException_Quando_Senha_Estiver_Nas_Ultimas_5_Utilizadas()
        {
            InstanciarSetup();
            var dto = new AlterarSenhaDto("23891476159", "Sgp@1234");

            mediator.Setup(x => x.Send(It.IsAny<VerificarUltimasSenhasQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

            var excecao = await Assert.ThrowsAsync<NegocioException>(
                async () => await alterarSenhaUseCase.Executar(dto, "Ab#12345"));

            Assert.Equal("A sua nova senha deve ser diferente das últimas 5 senhas utilizadas.", excecao.Message);
            mediator.Verify(x => x.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            mediator.Verify(x => x.Send(It.IsAny<AdicionarSenhaHistoricoCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory(DisplayName = "Deve Validar a Senha Anterior")]
        [InlineData("23891476159", "Ab#12345", "Ab12345")]
        [InlineData("09900791126", "Sgp@1234", "Ab#12346")]
        public async Task Deve_Validar_Senha_Anterior(string cpf, string senha, string senhaAnterior)
        {
            InstanciarSetup();

            var dto = new AlterarSenhaDto(cpf, senha);

            await Assert.ThrowsAsync<NegocioException>(async () => await alterarSenhaUseCase.Executar(dto, senhaAnterior));
        }

        [Theory(DisplayName = "Deve Acusar erro de validação")]
        [InlineData("", "", "Ab#12345")]
        [InlineData("238914767159", "Ab#12345", "Ab#12345")]
        [InlineData("09900791126", "Sgp1234", "Ab#12345")]
        [InlineData("61056974141", "", "Ab#12345")]
        [InlineData("", "#Aa12345", "Ab#12345")]
        [InlineData("23814767159", "Ab#12345", "Ab#12345")]
        [InlineData("00000100000", "Ab#12345", "Ab#12345")]
        public async Task Deve_Acusar_Erro_Validacao(string cpf, string senha, string senhaAnterior)
        {
            InstanciarSetup();

            var dto = new AlterarSenhaDto(cpf, senha);

            await Assert.ThrowsAsync<ValidacaoException>(async () => await alterarSenhaUseCase.Executar(dto, senhaAnterior));
        }

        private void InstanciarSetup()
        {
            mediator.Setup(x => x.Send(It.IsAny<VerificarUltimasSenhasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            mediator.Setup(x => x.Send(It.IsAny<AdicionarSenhaHistoricoCommand>(), It.IsAny<CancellationToken>()));
            mediator.Setup(x => x.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()));
            mediator.Setup(x => x.Send(It.IsAny<CriarTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(TokenMock);
            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new RetornoUsuarioCoreSSO
            {
                Cpf = "76565938105",
                UsuId = Guid.NewGuid(),
                TipoCriptografia = TipoCriptografia.TripleDES,
                Senha = "y05Om5MZcpi+QTomos5O2g==",
            });
        }
    }
}