using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.UseCase.UltimaAtualizacaoWorker;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.CasoDeUso
{
    public class ObterUltimaAtualizacaoPorProcessoUseCase : IObterUltimaAtualizacaoPorProcessoUseCase
    {
        private readonly IMediator mediator;

        public ObterUltimaAtualizacaoPorProcessoUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<UltimaAtualizaoWorkerPorProcessoResultado> Executar(string nomeProcesso)
        {
            return await mediator.Send(new ObterUltimaAtualizacaoPorProcessoQuery(nomeProcesso));
        }
    }
}
