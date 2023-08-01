using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterResponsaveisPorDreEUeQuery : IRequest<IEnumerable<ResponsavelAlunoEOLDto>>
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public int? AnoLetivo { get; set; }

        public ObterResponsaveisPorDreEUeQuery(string codigoDre, string codigoUe, int? anoLetivo = null)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
            AnoLetivo = anoLetivo;
        }
    }
}
