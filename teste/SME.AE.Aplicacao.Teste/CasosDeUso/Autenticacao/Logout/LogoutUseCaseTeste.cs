using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Autenticacao;
using SME.AE.Aplicacao.Comum.Modelos;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.Logout
{
    [Trait("Autenticacao", "Logout")]
    public class LogoutUseCaseTeste : AutenticacaoTesteBase
    {
        private static Mock<IMediator> Mediator(bool resposta)
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RemoveUsuarioDispositivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(resposta);
            return mediator;
        }

        [Fact]
        public async Task Deve_Retornar_Ok_Quando_Dispositivo_For_Removido()
        {
            var resultado = await ControllerComMediator(true).Logout("123", "disp");
            Assert.True(Assert.IsType<RespostaApi>(Assert.IsType<OkObjectResult>(resultado.Result).Value).Ok);
        }

        [Fact]
        public async Task Deve_Retornar_Ok_ComResposta_Falsa_Quando_Remocao_Falhar()
        {
            var resultado = await ControllerComMediator(false).Logout("123", "disp");
            Assert.False(Assert.IsType<RespostaApi>(Assert.IsType<OkObjectResult>(resultado.Result).Value).Ok);
        }

        [Fact]
        public async Task Deve_Encaminhar_Cpf_Ao_Mediator()
        {
            var mediator = Mediator(true);
            await ControllerComMediator(mediator).Logout("cpf-especifico", "disp");
            mediator.Verify(x => x.Send(It.Is<RemoveUsuarioDispositivoCommand>(c => c.Cpf == "cpf-especifico"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_Dispositivo_Ao_Mediator()
        {
            var mediator = Mediator(true);
            await ControllerComMediator(mediator).Logout("123", "dispositivo-especifico");
            mediator.Verify(x => x.Send(It.Is<RemoveUsuarioDispositivoCommand>(c => c.DispositivoId == "dispositivo-especifico"), It.IsAny<CancellationToken>()), Times.Once);
        }

        private SME.AE.Api.Controllers.AutenticacaoController ControllerComMediator(bool resposta) => ControllerComMediator(Mediator(resposta));
        private SME.AE.Api.Controllers.AutenticacaoController ControllerComMediator(Mock<IMediator> mediator)
        {
            var controller = Controller;
            controller.ControllerContext = ContextoComMediator(mediator.Object);
            return controller;
        }
    }
}
