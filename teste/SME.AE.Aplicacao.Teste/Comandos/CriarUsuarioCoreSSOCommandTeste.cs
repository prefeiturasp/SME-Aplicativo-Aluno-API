
using Moq;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Entrada;
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
            usuarioCoreSSORepositorio.Setup(x => x.Selecionar(It.IsAny<string>())).ReturnsAsync(new List<RetornoUsuarioCoreSSO>());

            usuarioCoreSSORepositorio.Setup(x => x.Criar(It.IsAny<UsuarioCoreSSODto>())).ReturnsAsync(Guid.NewGuid());

            usuarioCoreSSORepositorio.Setup(x => x.ObterPorId(It.IsAny<Guid>())).ReturnsAsync(It.IsAny<RetornoUsuarioCoreSSO>());

            await criarUsuarioCoreSSOCommand.Handle(new CriarUsuarioCoreSSOCommand
            {
                Usuario = new UsuarioCoreSSODto
                {
                    Cpf = "00000000000",
                    Grupos = new List<Guid>
                    {
                        Guid.NewGuid()
                    },
                    Nome = "Teste",
                    Senha = "Ab#12345"
                }
            }, new CancellationToken());
        }

        //[Fact(DisplayName = "Deve acusar usuário já existente")]
    }
}
