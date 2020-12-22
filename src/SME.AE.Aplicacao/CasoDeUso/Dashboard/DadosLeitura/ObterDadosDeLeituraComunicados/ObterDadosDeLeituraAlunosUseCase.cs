using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.Usuario.Dashboard;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterDadosDeLeituraAlunosUseCase : IObterDadosDeLeituraAlunosUseCase
    {

        private readonly IMediator mediator;

        public ObterDadosDeLeituraAlunosUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        }

        public Task<IEnumerable<DadosLeituraAlunosComunicado>> Executar(long notificacaoId, long codigoTurma)
        {
            return mediator.Send(new ObterDadosLeituraAlunosQuery(notificacaoId, codigoTurma));
        }
    }
}
