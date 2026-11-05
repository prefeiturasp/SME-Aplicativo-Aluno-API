using Microsoft.AspNetCore.Mvc;
using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comandos.CoreSSO.AssociarGrupoUsuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.Usuario;
using SME.AE.Aplicacao.Comandos.Usuario.AtualizaPrimeiroAcesso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Comum.Excecoes;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Autenticacao.PrimeiroAcesso
{
    [Trait("Autenticacao", "PrimeiroAcesso")]
    public class PrimeiroAcessoControllerTeste : AutenticacaoTesteBase
    {
        private readonly IPrimeiroAcessoUseCase criarUsuarioPrimeiroAcessoUseCase;
        public PrimeiroAcessoControllerTeste()
        {
            criarUsuarioPrimeiroAcessoUseCase = new PrimeiroAcessoUseCase(mediator.Object);
        }
        [Fact]
        public async Task Deve_Retornar_Ok_Em_Primeiro_Acesso()
        {
            var dto = new NovaSenhaDto { Id = 1, NovaSenha = "nova" };
            var resposta = Sucesso();
            var useCase = new Mock<IPrimeiroAcessoUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.PrimeiroAcesso(dto, useCase.Object);
            Assert.Same(resposta, Assert.IsType<OkObjectResult>(resultado.Result).Value);
        }

        [Fact]
        public async Task Deve_Retornar_Falha_Em_Primeiro_Acesso()
        {
            var dto = new NovaSenhaDto();
            var resposta = Falha();
            var useCase = new Mock<IPrimeiroAcessoUseCase>();
            useCase.Setup(x => x.Executar(dto)).ReturnsAsync(resposta);
            var resultado = await Controller.PrimeiroAcesso(dto, useCase.Object);
            Assert.Same(resposta, Assert.IsType<OkObjectResult>(resultado.Result).Value);
        }

        [Fact]
        public async Task Deve_Encaminhar_Id_Ao_Caso_De_Uso()
        {
            var dto = new NovaSenhaDto { Id = 42 };
            var useCase = new Mock<IPrimeiroAcessoUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<NovaSenhaDto>())).ReturnsAsync(Sucesso());
            await Controller.PrimeiroAcesso(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<NovaSenhaDto>(d => d.Id == 42)), Times.Once);
        }

        [Fact]
        public async Task Deve_Encaminhar_Nova_Senha_Ao_Caso_De_Uso()
        {
            var dto = new NovaSenhaDto { NovaSenha = "senha-especifica" };
            var useCase = new Mock<IPrimeiroAcessoUseCase>();
            useCase.Setup(x => x.Executar(It.IsAny<NovaSenhaDto>())).ReturnsAsync(Sucesso());
            await Controller.PrimeiroAcesso(dto, useCase.Object);
            useCase.Verify(x => x.Executar(It.Is<NovaSenhaDto>(d => d.NovaSenha == "senha-especifica")), Times.Once);
        }

        [Fact(DisplayName = "Deve Criar usuário no primeiro acesso")]
        public async Task Deve_Criar_Usuario_Primeiro_Acesso()
        {
            usuario.InserirAuditoria();

            InstanciarComandos();

            var resultado = await criarUsuarioPrimeiroAcessoUseCase.Executar(novaSenhaDtoCerta);
            Assert.True(resultado.Ok);
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

            mediator.Setup(a => a.Send(It.IsAny<ObterDadosResponsavelQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(responsavelAlunoEol);

            mediator.Setup(a => a.Send(It.IsAny<AssociarGrupoUsuarioCommand>(), It.IsAny<CancellationToken>()));

            mediator.Setup(a => a.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()));

            mediator.Setup(a => a.Send(It.IsAny<ObterDadosResponsavelResumidoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(responsavelAlunoEolResumido);
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

        private DadosResponsavelAluno[] responsavelAlunoEol { get; set; } = new DadosResponsavelAluno[]
        {
            new DadosResponsavelAluno {  NumeroCelular = "999999999",
                DDDCelular = "99",
                Email = "Teste@teste.com",
                Nome = "Teste"
            }
        };

        private DadosResponsavelAlunoResumido responsavelAlunoEolResumido { get; set; } = new DadosResponsavelAlunoResumido
        {
            NumeroCelular = "999999999",
            DDDCelular = "99",
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
