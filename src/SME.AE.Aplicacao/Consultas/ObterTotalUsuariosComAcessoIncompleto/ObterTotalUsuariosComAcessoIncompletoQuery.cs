using MediatR;

namespace SME.AE.Aplicacao.Consultas.ObterTotalUsuariosComAcessoIncompleto
{
    public class ObterTotalUsuariosComAcessoIncompletoQuery : IRequest<long>
    {
        public string CodigoDre { get; set; }

        public string CodigoUe { get; set; }

        public ObterTotalUsuariosComAcessoIncompletoQuery(string codigoDre, string codigoUe)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
        }
    }
}
