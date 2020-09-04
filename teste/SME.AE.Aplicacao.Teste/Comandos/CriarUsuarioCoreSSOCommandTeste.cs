using FluentValidation.TestHelper;
using Moq;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.Comandos
{
    public class CriarUsuarioCoreSSOCommandTeste : BaseTeste
    {
        private readonly Mock<IUsuarioCoreSSORepositorio> usuarioCoreSSORepositorio;
        private readonly CriarUsuarioCoreSSOCommandHandler criarUsuarioCoreSSOCommand;

        public CriarUsuarioCoreSSOCommandTeste()
        {
            usuarioCoreSSORepositorio = new Mock<IUsuarioCoreSSORepositorio>();
            criarUsuarioCoreSSOCommand = new CriarUsuarioCoreSSOCommandHandler(usuarioCoreSSORepositorio.Object);
        }

        [Fact(DisplayName = "Deve Criar Usuário")]
        public async Task Deve_Criar_Usuario()
        {
            usuarioCoreSSORepositorio.Setup(x => x.ObterPorCPF(It.IsAny<string>()));

            InjetarSetups();

            await criarUsuarioCoreSSOCommand.Handle(criarUsuarioCoreSSOCommandObj, new CancellationToken());
        }

        [Fact(DisplayName = "Deve informar usuário já existente")]
        public async Task Deve_Informar_Usuario_Existente()
        {
            usuarioCoreSSORepositorio.Setup(x => x.ObterPorCPF(It.IsAny<string>())).ReturnsAsync(new RetornoUsuarioCoreSSO());

            InjetarSetups();

            await Assert.ThrowsAsync<NegocioException>(async () => await criarUsuarioCoreSSOCommand.Handle(criarUsuarioCoreSSOCommandObj, new CancellationToken()));
        }

        [Fact(DisplayName = "Deve Validar Command")]
        public void Deve_Validar_Command()
        {
            var result = ExecutarValidacaoCommand();

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact(DisplayName = "Deve Informar Senha Inválida")]
        public void Deve_Informar_Senha_Invalida()
        {
            criarUsuarioCoreSSOCommandObj.Usuario.Senha = "";

            var result = ExecutarValidacaoCommand();

            result.ShouldHaveValidationErrorFor(x => x.Usuario.Senha);
        }

        [Fact(DisplayName = "Deve informar erro em todos os campos")]
        public void Deve_Informar_Erro_Todos_Campos()
        {
            criarUsuarioCoreSSOCommandObj.Usuario = new UsuarioCoreSSODto();

            var result = ExecutarValidacaoCommand();

            result.ShouldHaveValidationErrorFor(x => x.Usuario.Cpf);
            result.ShouldHaveValidationErrorFor(x => x.Usuario.Senha);
            result.ShouldHaveValidationErrorFor(x => x.Usuario.Nome);

            criarUsuarioCoreSSOCommandObj.Usuario = default;

            result = ExecutarValidacaoCommand();

            result.ShouldHaveValidationErrorFor(x => x.Usuario);
        }

        private TestValidationResult<CriarUsuarioCoreSSOCommand, CriarUsuarioCoreSSOCommand> ExecutarValidacaoCommand()
        {
            var validator = new CriarUsuarioCoreSSOCommandValidator();

            return validator.TestValidate(criarUsuarioCoreSSOCommandObj);
        }

        private void InjetarSetups()
        {
            usuarioCoreSSORepositorio.Setup(x => x.Criar(It.IsAny<UsuarioCoreSSODto>())).ReturnsAsync(Guid.NewGuid());

            usuarioCoreSSORepositorio.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(It.IsAny<RetornoUsuarioCoreSSO>());

            usuarioCoreSSORepositorio.Setup(x => x.SelecionarGrupos()).ReturnsAsync(new List<Guid> { Guid.NewGuid() });

            usuarioCoreSSORepositorio.Setup(x => x.IncluirUsuarioNosGrupos(It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>()));
        }

        private CriarUsuarioCoreSSOCommand criarUsuarioCoreSSOCommandObj { get; set; } = new CriarUsuarioCoreSSOCommand
        {
            Usuario = new UsuarioCoreSSODto
            {
                Cpf = "00000000000",
                Grupos = new List<Guid> { Guid.NewGuid() },
                Nome = "Teste",
                Senha = "Ab12345"
            }
        };
    }
}
