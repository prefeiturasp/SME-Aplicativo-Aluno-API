using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Dominio.Entidades
{
    public class EntidadeBase
    {
        public long Id { get; set; }
        public DateTime CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
        public string? CriadoRf { get; set; }
        public string? AlteradoRf { get; set; }

        public void AtualizarAuditoria()
        {
            AlteradoEm = DateTime.Now;
            //AlteradoPor = "Falta Implementação";
        }

        public void InserirAuditoria()
        {
            CriadoEm = DateTime.Now;
            //CriadoPor = "Falta Implementação";
        }
    }
}
