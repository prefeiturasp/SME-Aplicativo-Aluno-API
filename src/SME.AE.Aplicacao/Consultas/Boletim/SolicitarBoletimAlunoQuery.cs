using MediatR;

namespace SME.AE.Aplicacao
{
    public class SolicitarBoletimAlunoQuery : IRequest<bool>
    {
        public SolicitarBoletimAlunoQuery(string dreCodigo, string ueCodigo, int semestre, string turmaCodigo, int anoLetivo, int modalidadeCodigo, int modelo, string alunoCodigo)
        {
            DreCodigo = dreCodigo;
            UeCodigo = ueCodigo;
            Semestre = semestre;
            TurmaCodigo = turmaCodigo;
            AnoLetivo = anoLetivo;
            ModalidadeCodigo = modalidadeCodigo;
            Modelo = modelo;
            AlunoCodigo = alunoCodigo;
        }

        public string DreCodigo { get; set; }

        public string UeCodigo { get; set; }

        public int Semestre { get; set; }

        public string TurmaCodigo { get; set; }

        public int AnoLetivo { get; set; }

        public int ModalidadeCodigo { get; set; }

        public int Modelo { get; set; }

        public string AlunoCodigo { get; set; }
    }
}
