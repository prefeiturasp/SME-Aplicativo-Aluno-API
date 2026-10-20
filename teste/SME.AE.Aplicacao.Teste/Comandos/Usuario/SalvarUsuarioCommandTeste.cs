using FluentValidation.TestHelper;
using MediatR;
using Moq;
using SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class SalvarUsuarioCommandTeste : BaseTeste
    {
        private const string MensagemUsuarioObrigatorio = "È necessário informar um usuário";

        private readonly SalvarUsuarioCommandHandler handler;
        private readonly SalvarUsuarioCommandValidator validator;
        private readonly Mock<IUsuarioRepository> usuarioRepository;

        public SalvarUsuarioCommandTeste()
        {
            usuarioRepository = new Mock<IUsuarioRepository>();
            handler = new SalvarUsuarioCommandHandler(usuarioRepository.Object);
            validator = new SalvarUsuarioCommandValidator();
        }

        [Fact(DisplayName = "Deve salvar o usuário pelo repositório")]
        public async Task Deve_Salvar_O_Usuario_Pelo_Repositorio()
        {
            var usuario = CriarUsuario();
            usuarioRepository.Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Usuario>())).ReturnsAsync(usuario.Id);

            var resultado = await handler.Handle(new SalvarUsuarioCommand(usuario), new CancellationToken());

            Assert.Equal(Unit.Value, resultado);
            usuarioRepository.Verify(x => x.SalvarAsync(usuario), Times.Once);
        }

        [Fact(DisplayName = "Deve exigir o usuário")]
        public void Deve_Exigir_O_Usuario()
        {
            var resultado = validator.TestValidate(new SalvarUsuarioCommand(null));

            resultado.ShouldHaveValidationErrorFor(x => x.Usuario).WithErrorMessage(MensagemUsuarioObrigatorio);
        }

        [Fact(DisplayName = "Deve aceitar quando o usuário for informado")]
        public void Deve_Aceitar_Quando_O_Usuario_For_Informado()
        {
            var resultado = validator.TestValidate(new SalvarUsuarioCommand(CriarUsuario()));

            resultado.ShouldNotHaveAnyValidationErrors();
        }

        private static Dominio.Entidades.Usuario CriarUsuario()
        {
            return new Dominio.Entidades.Usuario
            {
                Id = 1,
                Cpf = "12345678900",
                CriadoEm = DateTime.Now.AddMonths(-1),
                AlteradoEm = DateTime.Now
            };
        }
    }
}
