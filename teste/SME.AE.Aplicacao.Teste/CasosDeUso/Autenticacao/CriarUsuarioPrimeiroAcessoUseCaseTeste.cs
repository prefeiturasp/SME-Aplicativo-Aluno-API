using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Dominio.Entidades;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao
{
    public class CriarUsuarioPrimeiroAcessoUseCaseTeste : BaseTeste
    {
        private readonly CriarUsuarioPrimeiroAcessoUseCase criarUsuarioPrimeiroAcessoUseCase;

        public CriarUsuarioPrimeiroAcessoUseCaseTeste()
        {
            criarUsuarioPrimeiroAcessoUseCase = new CriarUsuarioPrimeiroAcessoUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Deve Criar usuário no primeiro acesso")]
        public async Task Deve_Criar_Usuario_Primeiro_Acesso()
        {
            usuario.InserirAuditoria();

            InstanciarComandos();

            await criarUsuarioPrimeiroAcessoUseCase.Executar(novaSenhaDtoCerta);
        }

        private Usuario usuario => new Usuario
        {
            Celular = "00000000000",
            Cpf = "00000000000",
            Email = "a@a.com",
            Id = 1,
            Nome = "Teste",
            UltimoLogin = DateTime.Now
        };

        private void InstanciarComandos()
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

            mediator.Setup(a => a.Send(It.IsAny<CriarUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(retornoUsuarioCoreSSO);

            mediator.Setup(a => a.Send(It.IsAny<AtualizarPrimeiroAcessoCommand>(), It.IsAny<CancellationToken>()));
        }

        private RetornoUsuarioCoreSSO retornoUsuarioCoreSSO => new RetornoUsuarioCoreSSO
        {
            Cpf = "00000000000",
            GrupoId = Guid.NewGuid(),
            Senha = "Ab#12345",
            TipoCriptografia = Comum.Enumeradores.TipoCriptografia.SHA512,
            UsuId = Guid.NewGuid()
        };

        private NovaSenhaDto novaSenhaDtoCerta => new NovaSenhaDto
        {
            Id = 1,
            NovaSenha = "Ab#12345"
        };
    }
}
