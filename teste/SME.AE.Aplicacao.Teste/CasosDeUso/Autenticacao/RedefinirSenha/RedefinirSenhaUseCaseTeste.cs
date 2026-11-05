using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.RedefinirSenha
{
    [Trait("Autenticacao", "RedefinirSenha")]
    public class RedefinirSenhaUseCaseTeste : AutenticacaoTesteBase
    {
        [Fact]
        public async Task Deve_Retornar_Sucesso_Ao_Redefinir_Senha()
        {
            var dto = new RedefinirSenhaDto { Token = "token", Senha = "senha" };
            var resposta = Sucesso();
            var useCase = new Mock<IRedefinirSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.RedefinirSenha(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Ao_Redefinir_Senha()
        {
            var dto = new RedefinirSenhaDto();
            var resposta = Falha();
            var useCase = new Mock<IRedefinirSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.RedefinirSenha(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Encaminhar_Token_E_Senha()
        {
            var dto = new RedefinirSenhaDto { Token = "token-especifico", Senha = "senha-especifica" };
            var useCase = new Mock<IRedefinirSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<RedefinirSenhaDto>())).ReturnsAsync(Sucesso());
            await Controller.RedefinirSenha(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<RedefinirSenhaDto>(d => d.Token == "token-especifico" && d.Senha == "senha-especifica")), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_O_Mesmo_Dto()
        {
            var dto = new RedefinirSenhaDto();
            var useCase = new Mock<IRedefinirSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(Sucesso());
            await Controller.RedefinirSenha(dto, useCase.Object);
            useCase.Verify(x => x.Executar(dto), Times.Once);
        }
    }
}
