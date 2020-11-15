using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterDadosLeituraComunicados
{
    public class ObterDadosLeituraComunicadosQuery : IRequest<IEnumerable<DadosLeituraResultado>>
    {
        public string CodigoDre { get; set; }

        public string CodigoUe { get; set; }

        public ObterDadosLeituraComunicadosQuery(string codigoDre, string codigoUe)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
        }
    }
}
