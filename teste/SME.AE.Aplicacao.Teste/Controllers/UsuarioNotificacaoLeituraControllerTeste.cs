using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.AE.Api.Controllers;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Controllers
{
    public class UsuarioNotificacaoLeituraControllerTeste : BaseTeste
    {
        private const string CpfUsuario = "12345678900";
        private const long CodigoAluno = 1234567;
        private const long IdNotificacao = 10;
        private const long OutraNotificacao = 20;
        private const string SituacaoNaoLida = "Não lida";

        private readonly UsuarioNotificacaoLeituraController controller;
        private readonly Mock<IMarcarMensagemLidaUseCase> marcarMensagemLidaUseCase;
        private readonly Mock<IObterStatusDeLeituraNotificacaoUseCase> obterStatusDeLeituraNotificacaoUseCase;

        public UsuarioNotificacaoLeituraControllerTeste()
        {
            marcarMensagemLidaUseCase = new Mock<IMarcarMensagemLidaUseCase>();
            obterStatusDeLeituraNotificacaoUseCase = new Mock<IObterStatusDeLeituraNotificacaoUseCase>();

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

        [Fact(DisplayName = "Deve retornar 200 com os status de leitura das notificações do aluno")]
        public async Task Deve_Retornar_200_Com_Os_Status_De_Leitura_Das_Notificacoes_Do_Aluno()
        {
            var notificacoesId = new List<long> { IdNotificacao, OutraNotificacao };
            var statusLeitura = new List<StatusNotificacaoUsuario>
            {
                new StatusNotificacaoUsuario(IdNotificacao, null, SituacaoNaoLida),
                new StatusNotificacaoUsuario(OutraNotificacao, null, SituacaoNaoLida)
            };

            obterStatusDeLeituraNotificacaoUseCase.Setup(x => x.Executar(notificacoesId, CodigoAluno))
                                                  .ReturnsAsync(statusLeitura);

            var resultado = await controller.ObterStatusDeLeituraNotificacao(notificacoesId, CodigoAluno, obterStatusDeLeituraNotificacaoUseCase.Object);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Same(statusLeitura, okResult.Value);
            obterStatusDeLeituraNotificacaoUseCase.Verify(x => x.Executar(notificacoesId, CodigoAluno), Times.Once);
        }
    }
}
