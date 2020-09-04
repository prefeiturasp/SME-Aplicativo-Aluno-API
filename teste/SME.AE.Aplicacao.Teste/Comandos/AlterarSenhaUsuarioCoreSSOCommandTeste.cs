using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class AlterarSenhaUsuarioCoreSSOCommandTeste : BaseTeste
    {
        private readonly AlterarSenhaUsuarioCoreSSOCommandhandler alterarSenhaUsuarioCoreSSOCommandhandler;
        private readonly Mock<IUsuarioCoreSSORepositorio> usuarioCoreSSORepositorio;

        public AlterarSenhaUsuarioCoreSSOCommandTeste()
        {
            usuarioCoreSSORepositorio = new Mock<IUsuarioCoreSSORepositorio>();
            alterarSenhaUsuarioCoreSSOCommandhandler = new AlterarSenhaUsuarioCoreSSOCommandhandler(usuarioCoreSSORepositorio.Object);
        }

        [Fact(DisplayName = "Deve alterar senha")]
        public async Task Deve_Alterar_Senha()
        {
            InstanciarSetup();

            await alterarSenhaUsuarioCoreSSOCommandhandler.Handle(alterarSenhaUsuarioCoreSSOCommand, new CancellationToken());
        }

        [Fact(DisplayName = "Deve validar os validadores do comando")]
        public void Deve_Validar_Comando()
        {
            var validador = new AlterarSenhaUsuarioCoreSSOCommandValidator();

            var result = validador.TestValidate(alterarSenhaUsuarioCoreSSOCommand);

            result.ShouldNotHaveAnyValidationErrors();

            alterarSenhaUsuarioCoreSSOCommand.SenhaCriptograda = "";

            result = validador.TestValidate(alterarSenhaUsuarioCoreSSOCommand);

            result.ShouldHaveValidationErrorFor(x => x.SenhaCriptograda);

            alterarSenhaUsuarioCoreSSOCommand.SenhaCriptograda = "teste";
            alterarSenhaUsuarioCoreSSOCommand.UsuarioId = Guid.Empty;

            result = validador.TestValidate(alterarSenhaUsuarioCoreSSOCommand);

            result.ShouldHaveValidationErrorFor(x => x.UsuarioId);
        }

        private void InstanciarSetup()
        {
            usuarioCoreSSORepositorio.Setup(x => x.AlterarSenha(It.IsAny<Guid>(), It.IsAny<string>()));
        }

        private AlterarSenhaUsuarioCoreSSOCommand alterarSenhaUsuarioCoreSSOCommand { get; set; } = new AlterarSenhaUsuarioCoreSSOCommand(Guid.NewGuid(), "Teste@123");


    }
}
