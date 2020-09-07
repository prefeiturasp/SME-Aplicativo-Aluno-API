using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosPorDreUeCpfResponsavelQuery : IRequest<IEnumerable<AlunoRespostaEol>>
    {
        public ObterDadosAlunosPorDreUeCpfResponsavelQuery(string codigoDre, long codigoUe, string cpf)
        {
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
            Cpf = cpf;
        }

        public string CodigoDre { get; set; }
        public long CodigoUe { get; set; }
        public string Cpf { get; set; }
    }
}
