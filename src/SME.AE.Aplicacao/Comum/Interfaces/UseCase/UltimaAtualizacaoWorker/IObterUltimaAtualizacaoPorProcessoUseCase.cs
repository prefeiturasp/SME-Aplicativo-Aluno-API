using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.UseCase.UltimaAtualizacaoWorker
{
    public interface IObterUltimaAtualizacaoPorProcessoUseCase
    {
        Task<UltimaAtualizaoWorkerPorProcessoResultado> Executar(string nomeProcesso);
    }
}
