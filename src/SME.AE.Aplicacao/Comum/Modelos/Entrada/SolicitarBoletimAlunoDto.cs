namespace SME.AE.Aplicacao
{
    public class SolicitarBoletimAlunoDto
    {
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
