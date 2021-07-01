namespace SME.AE.Aplicacao
{
    public class NotaConceitoBimestreComponenteDto
    {
        public long Id { get; set; }
        public long ConselhoClasseNotaId { get; set; }
        public int? Bimestre { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public string ComponenteCurricularNome { get; set; }

        public string CorDaNota { get; set; }
        public long? ConceitoId { get; set; }
        public double Nota { get; set; }
        public string NotaConceito { get => ConceitoId.HasValue ? ObterConceito(ConceitoId.Value) : Nota.ToString(); }

        private string ObterConceito(long conceitoId)
        {
            if (conceitoId == 1)
                return "P";
            else if (conceitoId == 2)
                return "N";
            else return "NS";
        }
    }
}
