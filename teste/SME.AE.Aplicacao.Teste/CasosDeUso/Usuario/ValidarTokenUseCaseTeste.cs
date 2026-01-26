using Moq;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Usuario;
using SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao;
using SME.AE.Comum.Excecoes;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Usuario
{
    public class ValidarTokenUseCaseTeste : BaseTeste
    {
        private readonly Mock<IUsuarioRepository> usuarioRepository;
        private readonly ValidarTokenUseCase validarTokenUseCase;
        public ValidarTokenUseCaseTeste()
        {
            usuarioRepository = new Mock<IUsuarioRepository>();
            validarTokenUseCase = new ValidarTokenUseCase(mediator.Object);
        }

        [Fact(DisplayName = "Validar Token Valido")]
        public async Task Deve_Validar_Token()
        {
            InstanciarSetup();
            var usuario = await usuarioRepository.Object.ObterPorCpf("23891476159");
            
            var dto = new ValidarTokenDto { Token = usuario.Token };
            var tokenParaValidar =  await validarTokenUseCase.Executar(dto);
            Assert.True(tokenParaValidar.Ok);
            Assert.Empty(tokenParaValidar.Erros);

        }

        [Fact(DisplayName = "Validar Token Invalido")]
        public async Task Deve_acusar_erro_ao_validar_token_invalido()
        {
            InstanciarSetup();
            var tokenInvalidoParaTeste = "TOKEN_MUITO_INVALIDO";
            var dto = new ValidarTokenDto { Token = tokenInvalidoParaTeste };
            var usuarioComTokenDiferente = new Dominio.Entidades.Usuario
            {
                Id = 2,
                Cpf = "99999999999",
                Token = "TOKEN_CORRETO_MAS_DIFERENTE",
                ValidadeToken = DateTime.Now.AddDays(1),
                RedefinirSenha = true
            };

            mediator.Setup(x => x.Send(It.Is<ObterUsuarioPorTokenRedefinicaoQuery>(q => q.Token == tokenInvalidoParaTeste.ToUpper()), It.IsAny<CancellationToken>())).ReturnsAsync(usuarioComTokenDiferente);
            var exception = await Assert.ThrowsAsync<NegocioException>(async () =>
            {
                await validarTokenUseCase.Executar(dto);
            });
            Assert.Equal("Codigo de Verificação inválido", exception.Message);

        }

        private void InstanciarSetup()
        {
            var usuarioExistente = new Dominio.Entidades.Usuario
            {
                Id = 1,
                Cpf = "23891476159",
                UltimoLogin = DateTime.Now.AddDays(-5),
                CriadoEm = DateTime.Now.AddMonths(-1),
                AlteradoEm = DateTime.Now.AddDays(-5),
                Excluido = false,
                RedefinirSenha = true,
                Token = "TOKENANTIGO",
                ValidadeToken = DateTime.Now.AddDays(1)
            };
            usuarioRepository.Setup(x => x.SalvarAsync(It.IsAny<Dominio.Entidades.Usuario>())).ReturnsAsync(usuarioExistente.Id);
            usuarioRepository.Setup(x => x.ObterPorCpf(usuarioExistente.Cpf)).ReturnsAsync(usuarioExistente);
            usuarioRepository.Setup(x => x.ObterUsuarioPorTokenAutenticacao(usuarioExistente.Token.ToUpper())).ReturnsAsync(usuarioExistente);
            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioPorTokenRedefinicaoQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuarioExistente);
        }
    }
}
