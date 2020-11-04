using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;

namespace SME.AE.Aplicacao.Consultas.ObterUltimaAtualizacaoPorProcesso
{
    public class ObterUltimaAtualizacaoPorProcessoQuery : IRequest<UltimaAtualizaoWorkerPorProcessoResultado>
    {
        public string NomeProcesso { get; set; }

        public ObterUltimaAtualizacaoPorProcessoQuery(string nomeProcesso)
        {
            NomeProcesso = nomeProcesso;
        }
    }
}
