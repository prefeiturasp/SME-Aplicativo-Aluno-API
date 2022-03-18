using MediatR;

namespace SME.AE.Aplicacao
{
    public class SolicitarRelatorioRaaQuery : IRequest<bool>
    {
        public SolicitarRelatorioRaaQuery(int turmaId, string alunoCodigo, int semestre)
        {
            TurmaId = turmaId;
            AlunoCodigo = alunoCodigo;
            Semestre = semestre;
        }

        public int TurmaId { get; set; }
        public string AlunoCodigo { get; set; }
        public int Semestre { get; set; }
    }
}
