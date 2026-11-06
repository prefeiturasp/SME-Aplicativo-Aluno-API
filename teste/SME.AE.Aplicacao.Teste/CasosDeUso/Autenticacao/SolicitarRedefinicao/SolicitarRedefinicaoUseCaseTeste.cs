using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.SolicitarRedefinicao
{
    [Trait("Autenticacao", "SolicitarRedefinicao")]
    public class SolicitarRedefinicaoUseCaseTeste : AutenticacaoTesteBase
    {
        [Fact]
        public async Task Deve_Retornar_Sucesso_Ao_Solicitar_Token()
        {
            var dto = new GerarTokenDto { CPF = "123" };
            var resposta = Sucesso();
            var useCase = new Mock<ISolicitarRedifinicaoSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.SolicitarRedefinicao(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Ao_Solicitar_Token()
        {
            var dto = new GerarTokenDto();
            var resposta = Falha();
            var useCase = new Mock<ISolicitarRedifinicaoSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.SolicitarRedefinicao(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Encaminhar_Cpf_Ao_Caso_De_Uso()
        {
            var dto = new GerarTokenDto { CPF = "cpf-especifico" };
            var useCase = new Mock<ISolicitarRedifinicaoSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<GerarTokenDto>())).ReturnsAsync(Sucesso());
            await Controller.SolicitarRedefinicao(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<GerarTokenDto>(d => d.CPF == "cpf-especifico")), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_O_Mesmo_Dto()
        {
            var dto = new GerarTokenDto();
            var useCase = new Mock<ISolicitarRedifinicaoSenhaUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(Sucesso());
            await Controller.SolicitarRedefinicao(dto, useCase.Object);
            useCase.Verify(x => x.Executar(dto), Times.Once);
        }
    }
}
