using Moq;
using SME.AE.Aplicacao.CasoDeUso;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SME.AE.Aplicacao.Teste.CasosDeUso.UsuarioNotificacao
{
    public class ObterStatusDeLeituraNotificacaoUseCaseTeste : BaseTeste
    {
        private const long CodigoAluno = 1234567;
        private const long IdNotificacao = 10;
        private const long OutraNotificacao = 20;
        private const string SituacaoNaoLida = "Não lida";
        private const string SituacaoNaoTemApp = "Não tem APP";

        private readonly IObterStatusDeLeituraNotificacaoUseCase obterStatusDeLeituraNotificacaoUseCase;
        private readonly List<long> notificacoesId;

        public ObterStatusDeLeituraNotificacaoUseCaseTeste()
        {
            obterStatusDeLeituraNotificacaoUseCase = new ObterStatusDeLeituraNotificacaoUseCase(mediator.Object);
            notificacoesId = new List<long> { IdNotificacao, OutraNotificacao };
        }

        [Fact(DisplayName = "Deve consultar o status de leitura com as notificações e o aluno informados")]
        public async Task Deve_Consultar_O_Status_De_Leitura_Com_As_Notificacoes_E_O_Aluno_Informados()
        {
            mediator.Setup(m => m.Send(It.IsAny<ObterStatusNotificacaoUsuarioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<StatusNotificacaoUsuario>());

            await obterStatusDeLeituraNotificacaoUseCase.Executar(notificacoesId, CodigoAluno);

            mediator.Verify(m => m.Send(It.Is<ObterStatusNotificacaoUsuarioQuery>(q => q.NotificoesId == notificacoesId &&
                                                                                    q.CodigoAluno == CodigoAluno), It.IsAny<CancellationToken>()), Times.Once);
            mediator.Verify(m => m.Send(It.IsAny<ObterStatusNotificacaoUsuarioQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar os status obtidos pela consulta")]
        public async Task Deve_Retornar_Os_Status_Obtidos_Pela_Consulta()
        {
            var statusLeitura = new List<StatusNotificacaoUsuario>
            {
                new StatusNotificacaoUsuario(IdNotificacao, null, SituacaoNaoLida),
                new StatusNotificacaoUsuario(OutraNotificacao, null, SituacaoNaoTemApp)
            };
            mediator.Setup(m => m.Send(It.IsAny<ObterStatusNotificacaoUsuarioQuery>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(statusLeitura);

            var resultado = await obterStatusDeLeituraNotificacaoUseCase.Executar(notificacoesId, CodigoAluno);

            Assert.Same(statusLeitura, resultado);
        }
    }
}
