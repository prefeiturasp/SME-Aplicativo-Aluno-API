using System;

namespace SME.AE.Dominio.Comum
{
    public class EntidadeAuditavel
    {
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}