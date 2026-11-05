using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.ReiniciarSenha
{
    [Trait("Autenticacao", "ReiniciarSenha")]
    public class ReiniciarSenhaUseCaseTeste : AutenticacaoTesteBase
    {
        [Fact]
        public async Task Deve_Retornar_Sucesso_Ao_Reiniciar_Senha()
        {
            var dto = new SolicitarReiniciarSenhaDto { Cpf = "123" };
            var resposta = Sucesso();
            var useCase = new Mock<ISolicitarReiniciarSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.ReiniciarSenha(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Ao_Reiniciar_Senha()
        {
            var dto = new SolicitarReiniciarSenhaDto();
            var resposta = Falha();
            var useCase = new Mock<ISolicitarReiniciarSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.ReiniciarSenha(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Encaminhar_Cpf_Ao_Caso_De_Uso()
        {
            var dto = new SolicitarReiniciarSenhaDto { Cpf = "cpf-especifico" };
            var useCase = new Mock<ISolicitarReiniciarSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<SolicitarReiniciarSenhaDto>())).ReturnsAsync(Sucesso());
            await Controller.ReiniciarSenha(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<SolicitarReiniciarSenhaDto>(d => d.Cpf == "cpf-especifico")), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_O_Mesmo_Dto()
        {
            var dto = new SolicitarReiniciarSenhaDto();
            var useCase = new Mock<ISolicitarReiniciarSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(Sucesso());
            await Controller.ReiniciarSenha(dto, useCase.Object);
            useCase.Verify(x => x.Executar(dto), Times.Once);
        }
    }
}
