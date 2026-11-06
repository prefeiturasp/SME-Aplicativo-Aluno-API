using Moq;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.AlterarSenha
{
    [Trait("Autenticacao", "AlterarSenha")]
    public class AlterarSenhaUseCaseTeste : AutenticacaoTesteBase
    {
        [Fact]
        public async Task Deve_Retornar_Sucesso_Ao_Alterar_Senha()
        {
            var dto = new SenhaDto { SenhaAntiga = "antiga", NovaSenha = "nova" };
            var resposta = Sucesso();
            var useCase = new Mock<IAlterarSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<AlterarSenhaDto>(), dto.SenhaAntiga)).ReturnsAsync(resposta);
            var controller = Controller;
            controller.ControllerContext = ContextoComUsuario("usuario");
            var resultado = await controller.AlterarSenha(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Ao_Alterar_Senha()
        {
            var dto = new SenhaDto { SenhaAntiga = "antiga", NovaSenha = "nova" };
            var resposta = Falha();
            var useCase = new Mock<IAlterarSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<AlterarSenhaDto>(), dto.SenhaAntiga)).ReturnsAsync(resposta);
            var controller = Controller;
            controller.ControllerContext = ContextoComUsuario("usuario");
            var resultado = await controller.AlterarSenha(dto, useCase.Object);
            Assert.Same(resposta, resultado.Value);
        }

        [Fact]
        public async Task Deve_Usar_Nome_Do_Usuario_Como_Cpf()
        {
            var dto = new SenhaDto { NovaSenha = "nova" };
            var useCase = new Mock<IAlterarSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<AlterarSenhaDto>(), It.IsAny<string>())).ReturnsAsync(Sucesso());
            var controller = Controller;
            controller.ControllerContext = ContextoComUsuario("usuario-especifico");
            await controller.AlterarSenha(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<AlterarSenhaDto>(d => d.CPF == "usuario-especifico"), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_Senha_Nova_E_Antiga()
        {
            var dto = new SenhaDto { SenhaAntiga = "senha-antiga", NovaSenha = "senha-nova" };
            var useCase = new Mock<IAlterarSenhaUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<AlterarSenhaDto>(), It.IsAny<string>())).ReturnsAsync(Sucesso());
            var controller = Controller;
            controller.ControllerContext = ContextoComUsuario("usuario");
            await controller.AlterarSenha(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<AlterarSenhaDto>(d => d.Senha == "senha-nova"), "senha-antiga"), Times.Once);
        }
    }
}
