using MediatR;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterUltimaAtualizacaoPorProcessoQueryHandler : IRequestHandler<ObterUltimaAtualizacaoPorProcessoQuery, UltimaAtualizaoWorkerPorProcessoResultado>
    {
        private readonly IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio;

        public ObterUltimaAtualizacaoPorProcessoQueryHandler(IWorkerProcessoAtualizacaoRepositorio workerProcessoAtualizacaoRepositorio)
        {
            this.workerProcessoAtualizacaoRepositorio = workerProcessoAtualizacaoRepositorio ?? throw new System.ArgumentNullException(nameof(workerProcessoAtualizacaoRepositorio));
        }

        public async Task<UltimaAtualizaoWorkerPorProcessoResultado> Handle(ObterUltimaAtualizacaoPorProcessoQuery request, CancellationToken cancellationToken)
        {
            return await workerProcessoAtualizacaoRepositorio.ObterUltimaAtualizacaoPorProcesso(request.NomeProcesso);
        }
    }
}
