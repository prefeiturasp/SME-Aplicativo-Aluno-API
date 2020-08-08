using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class AssociarGrupoUsuarioCommandTeste : BaseTeste
    {
        private readonly Mock<IUsuarioCoreSSORepositorio> usuarioCoreSSORepositorio;
        private readonly AssociarGrupoUsuarioCommandHandler associarGrupoUsuarioCommandHandler;

        public AssociarGrupoUsuarioCommandTeste()
        {
            usuarioCoreSSORepositorio = new Mock<IUsuarioCoreSSORepositorio>();
            associarGrupoUsuarioCommandHandler = new AssociarGrupoUsuarioCommandHandler(usuarioCoreSSORepositorio.Object);
        }

        [Fact(DisplayName = "Deve apresentar grupos não encontrados")]
        public async Task Deve_Apresentar_Grupos_Nao_Encontrados()
        {
            await Assert.ThrowsAsync<NegocioException>(async () => await associarGrupoUsuarioCommandHandler.Handle(associarGrupoUsuarioCommand, new CancellationToken()));
        }

        [Fact(DisplayName = "Deve inserir grupos")]
        public async Task Deve_Inserir_Grupos()
        {
            InstanciarSetup();

            await associarGrupoUsuarioCommandHandler.Handle(associarGrupoUsuarioCommand, new CancellationToken());
        }

        [Fact(DisplayName = "Deve validar o validator do comando")]
        public void Deve_Validar_Validator()
        {
            var validator = new AssociarGrupoUsuarioCommandValidator();

            var result = validator.TestValidate(associarGrupoUsuarioCommand);

            result.ShouldNotHaveAnyValidationErrors();
        }

        private void InstanciarSetup()
        {
            usuarioCoreSSORepositorio.Setup(x => x.SelecionarGrupos()).ReturnsAsync(new List<Guid> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()});
            usuarioCoreSSORepositorio.Setup(x => x.IncluirUsuarioNosGrupos(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()));
        }

        private AssociarGrupoUsuarioCommand associarGrupoUsuarioCommand { get; set; } = new AssociarGrupoUsuarioCommand(
            new RetornoUsuarioCoreSSO { Grupos = new List<Guid> { Guid.Parse("6c48bae4-4740-47e1-a0bd-66a444b39d9e") } });
    }
}
