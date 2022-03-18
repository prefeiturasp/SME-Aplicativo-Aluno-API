using MediatR;

namespace SME.AE.Aplicacao
{
    public class SolicitarRelatorioRaaQuery : IRequest<bool>
    {
        public SolicitarRelatorioRaaQuery(string dreCodigo, string ueCodigo, int semestre, string turmaCodigo, int anoLetivo, int modalidadeCodigo, string alunoCodigo)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            Semestre = semestre;
            TurmaCodigo = turmaCodigo;
            AnoLetivo = anoLetivo;
            ModalidadeCodigo = modalidadeCodigo;
            AlunoCodigo = alunoCodigo;
        }

        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public int Semestre { get; set; }

        public string TurmaCodigo { get; set; }

        public int AnoLetivo { get; set; }

        public int ModalidadeCodigo { get; set; }

        public string AlunoCodigo { get; set; }
    }
}
