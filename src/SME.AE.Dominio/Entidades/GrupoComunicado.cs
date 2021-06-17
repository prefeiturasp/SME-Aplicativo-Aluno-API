namespace SME.AE.Dominio.Entidades
{
    public class GrupoComunicado : EntidadeBase
    {
        public string Nome { get; set; }
        public string TipoEscolaId { get; set; }
        public string TipoCicloId { get; set; }
        public string EtapaEnsinoId { get; set; }
        public bool Excluido { get; set; }
        public string CriadoRf { get; set; }
        public string AlteradoRf { get; set; }
    }
}
