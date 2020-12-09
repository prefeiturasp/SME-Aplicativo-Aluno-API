using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraModalidadeQuery : IRequest<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>>
    {
        public string CodigoDre { get; set; }

        public string CodigoUe { get; set; }

        public long NotificaoId { get; set; }

        public ModoVisualizacao ModoVisualizacao { get; set; }

        public ObterDadosLeituraModalidadeQuery(string codigoDre, string codigoUe, long notificaoId, ModoVisualizacao modoVisualizacao)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
            NotificaoId = notificaoId;
            ModoVisualizacao = modoVisualizacao;
        }
    }
}
