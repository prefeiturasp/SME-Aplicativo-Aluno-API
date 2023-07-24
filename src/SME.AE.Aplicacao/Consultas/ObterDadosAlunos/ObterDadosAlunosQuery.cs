using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas.ObterUsuario
{
    public class ObterDadosAlunosQuery : IRequest<IEnumerable<AlunoRespostaEol>>
    {

        public ObterDadosAlunosQuery(string cpfResponsavel, long? codigoAluno, string codigoDre, string codigoUe)
        {
            CpfResponsavel = cpfResponsavel;
            CodigoAluno = codigoAluno;
            CodigoDre = codigoDre;
            CodigoUe = codigoUe;
        }

        public string CpfResponsavel { get; set; }
        public long? CodigoAluno { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
    }
}
