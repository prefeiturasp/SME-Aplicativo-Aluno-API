using SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.NotasDoAluno
{
    public class NotaAlunoPorBimestreResposta
    {
        public const int BimestreDeFechamento = 0;

        public int AnoLetivo { get; set; }
        public string CodigoUe { get; set; }
        public string CodigoTurma { get; set; }
        public string AlunoCodigo { get; set; }
        public int Bimestre { get; set; }
        public string RecomendacoesFamilia { get; set; }
        public string RecomendacoesAluno { get; set; }
        public ICollection<NotaAlunoComponenteCurricular> NotasPorComponenteCurricular { get; set; }

        public NotaAlunoPorBimestreResposta()
        {
            NotasPorComponenteCurricular = new List<NotaAlunoComponenteCurricular>();
        }
    }
}