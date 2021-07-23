using System;

namespace SME.AE.Dominio.Entidades
{
    public abstract class  EntidadeBase
    {
        public long Id { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string AlteradoPor { get; set; }

        public void AtualizarAuditoria()
        {
            AlteradoEm = DateTime.Now;
            AlteradoPor = "Sistema";
        }

        public void InserirAuditoria()
        {
            CriadoEm = DateTime.Now;
            CriadoPor = "Sistema";
        }
    }
}
