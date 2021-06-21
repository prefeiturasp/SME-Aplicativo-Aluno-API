using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum.Excecoes;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao
{
    public class PrimeiroAcessoUseCaseTeste : BaseTeste
    {
        private readonly IPrimeiroAcessoUseCase criarUsuarioPrimeiroAcessoUseCase;

        public PrimeiroAcessoUseCaseTeste()
        {
            criarUsuarioPrimeiroAcessoUseCase = new PrimeiroAcessoUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Deve Criar usuário no primeiro acesso")]
        public async Task Deve_Criar_Usuario_Primeiro_Acesso()
        {
            usuario.InserirAuditoria();

            InstanciarComandos();

            await criarUsuarioPrimeiroAcessoUseCase.Executar(novaSenhaDtoCerta);
        }

        [Fact(DisplayName = "Deve Informar usuário não esta em primeiro acesso")]
        public async Task Deve_Informar_Usuario_Nao_Primeiro_Acesso()
        {
            usuario.InserirAuditoria();

            usuario.PrimeiroAcesso = false;

            await Assert.ThrowsAsync<NegocioException>(async () => await criarUsuarioPrimeiroAcessoUseCase.Executar(novaSenhaDtoCerta));
        }

        [Fact(DisplayName = "Deve Informar usuário não encontrado")]
        public async Task Deve_Informar_Usuario_Nao_Encontrado()
        {
            usuario.InserirAuditoria();

            usuario = null;

            InstanciarComandos();

            await Assert.ThrowsAsync<NegocioException>(async () => await criarUsuarioPrimeiroAcessoUseCase.Executar(novaSenhaDtoCerta));
        }
        
        private void InstanciarComandos() 
        {
            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuario);

            mediator.Setup(a => a.Send(It.IsAny<CriarUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(retornoUsuarioCoreSSO);

            mediator.Setup(a => a.Send(It.IsAny<AtualizarPrimeiroAcessoCommand>(), It.IsAny<CancellationToken>()));

            mediator.Setup(a => a.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(retornoUsuarioCoreSSO);

            mediator.Setup(a => a.Send(It.IsAny<ObterDadosResumidosReponsavelPorCpfQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(responsavelAlunoEolResumido);

            mediator.Setup(a => a.Send(It.IsAny<AssociarGrupoUsuarioCommand>(), It.IsAny<CancellationToken>()));

            mediator.Setup(a => a.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()));
        }

        private Dominio.Entidades.Usuario usuario { get; set; } = new Dominio.Entidades.Usuario
        {
            Cpf = "00000000000",         
            Id = 1,
            PrimeiroAcesso = true,
            UltimoLogin = DateTime.Now
        };

        private RetornoUsuarioCoreSSO retornoUsuarioCoreSSO { get; set; } = new RetornoUsuarioCoreSSO
        {
            Cpf = "00000000000",
            Grupos = new List<Guid> { Guid.NewGuid() },
            Senha = "Ab#12345",
            TipoCriptografia = Comum.Enumeradores.TipoCriptografia.SHA512,
            UsuId = Guid.NewGuid()
        };

        private ResponsavelAlunoEolResumidoDto responsavelAlunoEolResumido { get; set; } = new ResponsavelAlunoEolResumidoDto
        {
            Celular = "999999999",
            DDD = "99",
            Email = "Teste@teste.com",
            Nome = "Teste"
        };

        private NovaSenhaDto novaSenhaDtoCerta { get; set; } = new NovaSenhaDto
        {
            Id = 1,
            NovaSenha = "Ab12345"
        };
    }
}
