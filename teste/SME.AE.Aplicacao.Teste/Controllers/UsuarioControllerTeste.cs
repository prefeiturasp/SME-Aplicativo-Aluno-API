using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.AE.Api.Controllers;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Controllers
{
    public class UsuarioControllerTeste : BaseTeste
    {
        private const int StatusCodeFalhaAtualizacao = 601;

        private readonly UsuarioController controller;
        private readonly Mock<IAtualizarDadosUsuarioUseCase> atualizarDadosUsuarioUseCase;

        public UsuarioControllerTeste()
        {
            atualizarDadosUsuarioUseCase = new Mock<IAtualizarDadosUsuarioUseCase>();
            controller = new UsuarioController();
        }

        [Fact(DisplayName = "Deve retornar 200 com a resposta quando a atualização for bem-sucedida")]
        public async Task Deve_Retornar_200_Com_A_Resposta_Quando_A_Atualizacao_For_Bem_Sucedida()
        {
            var usuarioDto = CriarAtualizarDadosUsuarioDto();
            var resposta = RespostaApi.Sucesso();

            atualizarDadosUsuarioUseCase.Setup(x => x.Executar(usuarioDto)).ReturnsAsync(resposta);

            var resultado = await controller.AtualizarDadosUsuario(usuarioDto, atualizarDadosUsuarioUseCase.Object);

            var okResult = Assert.IsType<OkObjectResult>(resultado);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.Same(resposta, okResult.Value);
            atualizarDadosUsuarioUseCase.Verify(x => x.Executar(usuarioDto), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar 601 com a resposta quando a atualização falhar")]
        public async Task Deve_Retornar_601_Com_A_Resposta_Quando_A_Atualizacao_Falhar()
        {
            var usuarioDto = CriarAtualizarDadosUsuarioDto();
            var resposta = RespostaApi.Falha("Usuário não encontrado!");

            atualizarDadosUsuarioUseCase.Setup(x => x.Executar(usuarioDto)).ReturnsAsync(resposta);

            var resultado = await controller.AtualizarDadosUsuario(usuarioDto, atualizarDadosUsuarioUseCase.Object);

            Assert.IsNotType<OkObjectResult>(resultado);
            Assert.Equal(StatusCodeFalhaAtualizacao, resultado.StatusCode);
            Assert.Same(resposta, resultado.Value);
            atualizarDadosUsuarioUseCase.Verify(x => x.Executar(usuarioDto), Times.Once);
        }

        private static AtualizarDadosUsuarioDto CriarAtualizarDadosUsuarioDto()
        {
            return new AtualizarDadosUsuarioDto
            {
                Id = 1,
                Email = "responsavel@teste.com",
                NomeMae = "Maria da Silva",
                Celular = "(11) 98765-4321",
                DataNascimentoResponsavel = new DateTime(1985, 5, 10)
            };
        }
    }
}
