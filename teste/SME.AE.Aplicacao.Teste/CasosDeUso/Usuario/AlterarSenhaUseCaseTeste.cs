using Moq;
using SME.AE.Aplicacao.CasoDeUso.Usuario;
using SME.AE.Aplicacao.Comandos.CoreSSO.AdicionarSenhaHistorico;
using SME.AE.Aplicacao.Comandos.CoreSSO.AlterarSenhaUsuarioCoreSSO;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Excecoes;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO;
using SME.AE.Aplicacao.Consultas.VerificarSenha;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.Usuario
{
    public class AlterarSenhaUseCaseTeste : BaseTeste
    {
        private readonly IAlterarSenhaUseCase alterarSenhaUseCase;

        public AlterarSenhaUseCaseTeste()
        {
            alterarSenhaUseCase = new AlterarSenhaUseCase(mediator.Object);
        }

        [Theory(DisplayName = "Deve Alterar a Senha")]
        [InlineData("23891476159", "Ab#12345")]
        [InlineData("09900791126", "Sgp@1234")]
        [InlineData("61056974141", "Aa!11111")]
        [InlineData("77083696144", "#Aa12345")]
        [InlineData("49072009193", "Ab#12345")]
        [InlineData("95461504108", "Sgp@1234")]
        [InlineData("91063385180", "Aa!11111")]
        [InlineData("94377608100", "#Aa12345")]
        public async Task Deve_Alterar_Senha(string cpf, string senha)
        {
            InstanciarSetup();

            var dto = new AlterarSenhaDto(cpf, senha);

            await alterarSenhaUseCase.Executar(dto);
        }

        [Theory(DisplayName = "Deve Acusar erro de validação")]
        [InlineData("", "")]
        [InlineData("238914767159", "Ab#12345")]
        [InlineData("09900791126", "Sgp1234")]
        [InlineData("61056974141", "")]
        [InlineData("", "#Aa12345")]
        [InlineData("23814767159", "Ab#12345")]
        [InlineData("00000100000", "Ab#12345")]
        public async Task Deve_Acusar_Erro_Validacao(string cpf, string senha)
        {
            InstanciarSetup();

            var dto = new AlterarSenhaDto(cpf, senha);

            await Assert.ThrowsAsync<ValidacaoException>(async () => await alterarSenhaUseCase.Executar(dto));
        }

        private void InstanciarSetup()
        {
            mediator.Setup(x => x.Send(It.IsAny<VerificarUltimasSenhasQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
            mediator.Setup(x => x.Send(It.IsAny<AdicionarSenhaHistoricoCommand>(), It.IsAny<CancellationToken>()));
            mediator.Setup(x => x.Send(It.IsAny<AlterarSenhaUsuarioCoreSSOCommand>(), It.IsAny<CancellationToken>()));
            mediator.Setup(x => x.Send(It.IsAny<ObterUsuarioCoreSSOQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new RetornoUsuarioCoreSSO
            {
                Cpf = "76565938105",
                UsuId = Guid.NewGuid(),
                TipoCriptografia = TipoCriptografia.TripleDES
            });
        }
    }
}
