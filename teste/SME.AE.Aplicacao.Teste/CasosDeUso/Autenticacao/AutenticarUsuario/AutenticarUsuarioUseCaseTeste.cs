using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.AutenticarUsuario
{
    [Trait("Autenticacao", "AutenticarUsuario")]
    public class AutenticarUsuarioUseCaseTeste : AutenticacaoTesteBase
    {
        private readonly AutenticarUsuarioUseCase autenticarUsuarioUseCase;
        public AutenticarUsuarioUseCaseTeste()
        {
            autenticarUsuarioUseCase = new AutenticarUsuarioUseCase(mediator.Object);
        }
        [Fact]
        public async Task Deve_Retornar_Ok_Quando_Autenticacao_For_Bem_Sucedida()
        {
            var dto = new AutenticacaoDTO { Cpf = "123", Senha = "senha", DispositivoId = "disp" };
            var resposta = Sucesso();
            var useCase = new Mock<IAutenticarUsuarioUseCase>();
            useCase.Setup(x => x.Executar(dto.Cpf, dto.Senha, dto.DispositivoId)).ReturnsAsync(resposta);

            var resultado = await Controller.AutenticarUsuario(dto, useCase.Object);

            Assert.Same(resposta, Assert.IsType<OkObjectResult>(resultado.Result).Value);
        }

        [Fact]
        public async Task Deve_Retornar_BadRequest_Quando_Autenticacao_Falhar()
        {
            var dto = new AutenticacaoDTO { Cpf = "123", Senha = "senha", DispositivoId = "disp" };
            var resposta = Falha();
            var useCase = new Mock<IAutenticarUsuarioUseCase>();
            useCase.Setup(x => x.Executar(dto.Cpf, dto.Senha, dto.DispositivoId)).ReturnsAsync(resposta);

            var resultado = await Controller.AutenticarUsuario(dto, useCase.Object);

            Assert.Same(resposta, Assert.IsType<BadRequestObjectResult>(resultado.Result).Value);
        }

        [Fact]
        public async Task Deve_Encaminhar_Cpf_E_Senha_Ao_Caso_De_Uso()
        {
            var dto = new AutenticacaoDTO { Cpf = "cpf-especifico", Senha = "senha-especifica", DispositivoId = "disp" };
            var useCase = new Mock<IAutenticarUsuarioUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(Sucesso());

            await Controller.AutenticarUsuario(dto, useCase.Object);

            useCase.Verify(x => x.Executar("cpf-especifico", "senha-especifica", "disp"), Times.Once);
        }

        [Fact]
        public async Task Deve_Retornar_A_Mesma_Resposta_Do_Caso_De_Uso()
        {
            var dto = new AutenticacaoDTO();
            var resposta = RespostaApi.Falha("erro detalhado");
            var useCase = new Mock<IAutenticarUsuarioUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(resposta);

            var resultado = await Controller.AutenticarUsuario(dto, useCase.Object);

            Assert.Same(resposta, Assert.IsType<BadRequestObjectResult>(resultado.Result).Value);
        }

        [Fact(DisplayName = "Deve Autenticar Usuário")]
        public async Task Deve_Autenticar_Usuario()
        {
            mediator.Setup(a => a.Send(It.IsAny<AutenticarUsuarioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(RespostaApi.Sucesso(new RespostaAutenticar()));

            mediator.Setup(a => a.Send(It.IsAny<CriarTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("");

            mediator.Setup(a => a.Send(It.IsAny<UsuarioDispositivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var autenticacao = await autenticarUsuarioUseCase.Executar("000.000.000-00", "Ab#123456", "Teste");

            Assert.True(autenticacao.Ok);
        }
    }
}
