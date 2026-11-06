using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.ObterSituacaoUsuario
{
    [Trait("Autenticacao", "ObterSituacaoUsuario")]
    public class ObterSituacaoUsuarioUseCaseTeste : AutenticacaoTesteBase
    {
        [Fact]
        public async Task Deve_Retornar_True_Quando_Usuario_For_Responsavel()
        {
            var useCase = new Mock<IValidarUsuarioEhResponsavelDeAlunoUseCase>();
            useCase.Setup(x => x.Executar("123")).ReturnsAsync(true);
            var resultado = await Controller.ObterSituacaoUsuario("123", useCase.Object);
            Assert.True(Assert.IsType<bool>(resultado.Value));
        }

        [Fact]
        public async Task Deve_Retornar_False_Quando_Usuario_Nao_For_Responsavel()
        {
            var useCase = new Mock<IValidarUsuarioEhResponsavelDeAlunoUseCase>();
            useCase.Setup(x => x.Executar("123")).ReturnsAsync(false);
            var resultado = await Controller.ObterSituacaoUsuario("123", useCase.Object);
            Assert.False(Assert.IsType<bool>(resultado.Value));
        }

        [Fact]
        public async Task Deve_Retornar_Status_Ok()
        {
            var useCase = new Mock<IValidarUsuarioEhResponsavelDeAlunoUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<string>())).ReturnsAsync(true);
            var resultado = await Controller.ObterSituacaoUsuario("123", useCase.Object);
            Assert.Equal(200, resultado.StatusCode);
        }

        [Fact]
        public async Task Deve_Encaminhar_Cpf_Ao_Caso_De_Uso()
        {
            var useCase = new Mock<IValidarUsuarioEhResponsavelDeAlunoUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<string>())).ReturnsAsync(false);
            await Controller.ObterSituacaoUsuario("cpf-especifico", useCase.Object);
            useCase.Verify(x => x.Executar("cpf-especifico"), Times.Once);
        }
    }
}
