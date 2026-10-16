using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.AE.Api.Controllers;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Controllers
{
    public class UsuarioNotificacaoLeituraControllerTeste : BaseTeste
    {
        private const string CpfUsuario = "12345678900";

        private readonly UsuarioNotificacaoLeituraController controller;
        private readonly Mock<IMarcarMensagemLidaUseCase> marcarMensagemLidaUseCase;

        public UsuarioNotificacaoLeituraControllerTeste()
        {
            marcarMensagemLidaUseCase = new Mock<IMarcarMensagemLidaUseCase>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.Setup(s => s.GetService(typeof(IMediator))).Returns(mediator.Object);

            controller = new UsuarioNotificacaoLeituraController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        RequestServices = serviceProvider.Object,
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, CpfUsuario) }, "Bearer"))
                    }
                }
            };
        }

        [Fact(DisplayName = "Deve marcar mensagem como lida para o usuário logado e retornar 200")]
        public async Task Deve_Marcar_Mensagem_Como_Lida_Para_Usuario_Logado()
        {
            var usuarioNotificacaoDto = new UsuarioNotificacaoDto
            {
                NotificacaoId = 10,
                UsuarioId = 1,
                CodigoAlunoEol = 1234567,
                MensagemVisualizada = true
            };
            var notificacao = new NotificacaoResposta { Id = 10, Titulo = "Teste" };

            marcarMensagemLidaUseCase.Setup(x => x.Executar(mediator.Object, usuarioNotificacaoDto, CpfUsuario))
                                     .ReturnsAsync(notificacao);

            var resultado = await controller.MarcarMensagemLida(usuarioNotificacaoDto, marcarMensagemLidaUseCase.Object);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Same(notificacao, okResult.Value);
            marcarMensagemLidaUseCase.Verify(x => x.Executar(mediator.Object, usuarioNotificacaoDto, CpfUsuario), Times.Once);
        }
    }
}
