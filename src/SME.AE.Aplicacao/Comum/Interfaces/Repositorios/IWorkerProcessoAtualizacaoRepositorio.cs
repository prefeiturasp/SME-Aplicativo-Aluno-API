using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios
{
    public interface IWorkerProcessoAtualizacaoRepositorio
    {
        Task IncluiOuAtualizaUltimaAtualizacao(string nomeProcesso);

        Task<UltimaAtualizaoWorkerPorProcessoResultado> ObterUltimaAtualizacaoPorProcesso(string nomeProcesso);
    }
}
