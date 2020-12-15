using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraTurmaQuery : IRequest<IEnumerable<DadosLeituraComunicadosPorModalidadeTurmaResultado>>
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public long NotificaoId { get; set; }
        public short[] Modalidades { get; set; }
        public long[] CodigosTurmas { get; set; }
        public ModoVisualizacao ModoVisualizacao { get; set; }

        public ObterDadosLeituraTurmaQuery(string codigoDre, string codigoUe, long notificaoId, short[] modalidades, long[] codigosTurmas, ModoVisualizacao modoVisualizacao)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
            NotificaoId = notificaoId;
            ModoVisualizacao = modoVisualizacao;
            Modalidades = modalidades;
            CodigosTurmas = codigosTurmas;
        }
    }
}
