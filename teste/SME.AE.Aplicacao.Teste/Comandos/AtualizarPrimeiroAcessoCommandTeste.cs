using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class AtualizarPrimeiroAcessoCommandTeste : BaseTeste
    {
        private readonly AtualizarPrimeiroAcessoCommandHandler atualizarPrimeiroAcessoCommandHandler;
        private readonly Mock<IUsuarioRepository> usuarioRepository;

        public AtualizarPrimeiroAcessoCommandTeste()
        {
            usuarioRepository = new Mock<IUsuarioRepository>();

            atualizarPrimeiroAcessoCommandHandler = new AtualizarPrimeiroAcessoCommandHandler(usuarioRepository.Object);
        }

        [Fact(DisplayName = "Deve atualizar primeiro acesso")]
        public async Task Deve_Atualizar_Primeiro_Acesso()
        {
            InjetarSetups();

            await atualizarPrimeiroAcessoCommandHandler.Handle(atualizarPrimeiroAcessoCommand, new CancellationToken());
        }

        [Fact(DisplayName = "Deve acusar usuário não encontrado")]
        public async Task Deve_Acusar_Usuario_Nao_Encontrado()
        {
            await Assert.ThrowsAsync<NegocioException>(async () => await atualizarPrimeiroAcessoCommandHandler.Handle(atualizarPrimeiroAcessoCommand, new CancellationToken()));
        }

        [Fact(DisplayName = "Deve validar id")]
        public void Deve_Validar_Id()
        {
            var validador = new AtualizarPrimeiroAcessoCommandValidator();

            var result = validador.TestValidate(atualizarPrimeiroAcessoCommand);

            result.ShouldNotHaveAnyValidationErrors();

            atualizarPrimeiroAcessoCommand.Id = 0;

            result = validador.TestValidate(atualizarPrimeiroAcessoCommand);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        private void InjetarSetups()
        {
            usuarioRepository.Setup(x => x.ObterPorIdAsync(It.IsAny<long>())).ReturnsAsync(new Usuario());

            usuarioRepository.Setup(x => x.AtualizarPrimeiroAcesso(It.IsAny<long>(), It.IsAny<bool>()));
        }

        private AtualizarPrimeiroAcessoCommand atualizarPrimeiroAcessoCommand { get; set; } = new AtualizarPrimeiroAcessoCommand
        {
            Id = 1,
            PrimeiroAcesso = true
        };
    }
}
