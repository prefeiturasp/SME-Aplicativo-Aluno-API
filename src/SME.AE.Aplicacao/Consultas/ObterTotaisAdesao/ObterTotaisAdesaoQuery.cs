using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterTotaisAdesao
{
    public class ObterTotaisAdesaoQuery : IRequest<IEnumerable<TotaisAdesaoResultado>>
    {
        public string CodigoDre { get; set; }

        public string CodigoUe { get; set; }

        public ObterTotaisAdesaoQuery(string codigoDre, string codigoUe)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
        }
    }
}
