using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.ValidarToken
{
    [Trait("Autenticacao", "ValidarToken")]
    public class ValidarTokenUseCaseTeste : AutenticacaoTesteBase
    {
        [Fact]
        public async Task Deve_Retornar_Sucesso_Ao_Validar_Token()
        {
            var dto = new ValidarTokenDto { Token = "token" };
            var resposta = Sucesso();
            var useCase = new Mock<IValidarTokenUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.ValidarToken(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Ao_Validar_Token()
        {
            var dto = new ValidarTokenDto { Token = "invalido" };
            var resposta = Falha();
            var useCase = new Mock<IValidarTokenUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.ValidarToken(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Encaminhar_Token_Ao_Caso_De_Uso()
        {
            var dto = new ValidarTokenDto { Token = "token-especifico" };
            var useCase = new Mock<IValidarTokenUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<ValidarTokenDto>())).ReturnsAsync(Sucesso());
            await Controller.ValidarToken(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<ValidarTokenDto>(d => d.Token == "token-especifico")), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_O_Mesmo_Dto()
        {
            var dto = new ValidarTokenDto();
            var useCase = new Mock<IValidarTokenUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(Sucesso());
            await Controller.ValidarToken(dto, useCase.Object);
            useCase.Verify(x => x.Executar(dto), Times.Once);
        }
    }
}
