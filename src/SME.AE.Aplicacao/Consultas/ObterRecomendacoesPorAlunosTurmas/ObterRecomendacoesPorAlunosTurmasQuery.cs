using MediatR;
using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Modelos.Resposta;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Consultas
{
    public class ObterRecomendacoesPorAlunosTurmasQuery : IRequest<IEnumerable<RecomendacaoConselhoClasseAluno>>
    {
        public ObterRecomendacoesPorAlunosTurmasQuery(string codigoAluno, string codigoTurma, int anoLetivo, ModalidadeDeEnsino modalidade, int semestre)
        {
            CodigoAluno = codigoAluno;
            CodigoTurma = codigoTurma;
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
        }

        public string CodigoAluno { get; set; }
        public string CodigoTurma { get; set; }
        public int AnoLetivo { get; set; }
        public ModalidadeDeEnsino Modalidade { get; set; }
        public int Semestre { get; set; }
    }
}
