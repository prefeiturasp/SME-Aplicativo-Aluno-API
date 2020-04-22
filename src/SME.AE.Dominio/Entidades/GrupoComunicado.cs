using System;

namespace SME.AE.Dominio.Entidades
{
    public class GrupoComunicado
    {
        public long Id { get; set; }
        public String Nome { get; set; }
        public String TipoEscolaId { get; set; }
        public String TipoCicloId { get; set; }
        public DateTime CriadoEm { get; set; }
        public String CriadoPor { get; set; }
        public DateTime AlteradoEm { get; set; }
        public String AlteradoPor { get; set; }
        public String CriadoRf { get; set; }
        public String AlteradoRf { get; set; }
        public bool Excluido { get; set; }
    }
}
