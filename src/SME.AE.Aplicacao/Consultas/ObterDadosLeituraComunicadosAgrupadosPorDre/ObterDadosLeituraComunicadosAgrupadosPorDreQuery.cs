using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosAgrupadosPorDreQuery : IRequest<IEnumerable<DadosLeituraComunicadosResultado>>
    {
        public long NotificaoId { get; set; }

        public ModoVisualizacao ModoVisualizacao { get; set; }

        public ObterDadosLeituraComunicadosAgrupadosPorDreQuery(long notificaoId, ModoVisualizacao modoVisualizacao)
        {
            NotificaoId = notificaoId;
            ModoVisualizacao = modoVisualizacao;
        }
    }
}
