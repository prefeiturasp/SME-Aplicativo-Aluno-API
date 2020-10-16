using MediatR;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosValidos
{
    public class ObterTotalUsuariosValidosQuery : IRequest<long>
    {
        public string CodigoDre { get; set; }

        public string CodigoUe { get; set; }

        public ObterTotalUsuariosValidosQuery(string codigoDre, string codigoUe)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
        }
    }
}
