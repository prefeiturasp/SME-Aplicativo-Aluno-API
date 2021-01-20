using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterCpfsDeResponsaveisPorDreEUeQuery : IRequest<IEnumerable<CpfResponsavelAlunoEol>>
    {
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }

        public ObterCpfsDeResponsaveisPorDreEUeQuery(string codigoDre, string codigoUe)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
        }
    }
}
