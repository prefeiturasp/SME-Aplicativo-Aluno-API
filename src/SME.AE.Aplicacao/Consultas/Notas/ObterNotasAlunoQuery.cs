using MediatR;
using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;

namespace SME.AE.Aplicacao.Consultas.Notas
{
    public class ObterNotasAlunoQuery : IRequest<NotaAlunoPorBimestreResposta>
    {
        public int AnoLetivo { get; set; }
        public short Bimestre { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string CodigoAluno { get; set; }

        public ObterNotasAlunoQuery(int anoLetivo, short bimestre, string codigoUe, string codigoTurma, string codigoAluno)
        {
            AnoLetivo = anoLetivo;
            Bimestre = bimestre;
            CodigoUe = codigoUe;
            CodigoTurma = codigoTurma;
            CodigoAluno = codigoAluno;
        }
    }
}