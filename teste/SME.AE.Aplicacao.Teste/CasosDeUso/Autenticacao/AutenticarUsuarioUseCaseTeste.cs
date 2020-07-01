using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.Autenticacao.AutenticarUsuario;
using SME.AE.Aplicacao.Comandos.Token.Criar;
using SME.AE.Aplicacao.Comandos.Usuario.InseriDispositivo;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao
{
    public class AutenticarUsuarioUseCaseTeste : BaseTeste
    {
        private readonly AutenticarUsuarioUseCase autenticarUsuarioUseCase;
        public AutenticarUsuarioUseCaseTeste()
        {
            autenticarUsuarioUseCase = new AutenticarUsuarioUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Deve Autenticar Usuário")]
        public async Task Deve_Autenticar_Usuario()
        {
            mediator.Setup(a => a.Send(It.IsAny<AutenticarUsuarioCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(RespostaApi.Sucesso(new RespostaAutenticar()));

            mediator.Setup(a => a.Send(It.IsAny<CriarTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync("");

            mediator.Setup(a => a.Send(It.IsAny<UsuarioDispositivoCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var autenticacao = await autenticarUsuarioUseCase.Executar("000.000.000-00", "Ab#123456","Teste");

            Assert.True(autenticacao.Ok);
        }
    }
}
