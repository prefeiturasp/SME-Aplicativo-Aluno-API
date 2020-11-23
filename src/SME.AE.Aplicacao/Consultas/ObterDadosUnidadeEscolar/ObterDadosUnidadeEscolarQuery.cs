using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar;

namespace SME.AE.Aplicacao.Consultas.UnidadeEscolar
{
    public class ObterDadosUnidadeEscolarQuery : IRequest<UnidadeEscolarResposta>
    {
        public string CodigoUe { get; set; }

        public ObterDadosUnidadeEscolarQuery(string codigoUe)
        {
            CodigoUe = codigoUe;
        }
    }
}