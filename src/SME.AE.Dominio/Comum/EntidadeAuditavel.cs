using System;

namespace SME.AE.Dominio.Comum
{
    public class EntidadeAuditavel
    {
        public long Id { get; set; }
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}