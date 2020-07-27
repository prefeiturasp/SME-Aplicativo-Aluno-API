using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Dominio.Entidades
{
    public class NotificacaoAluno 
    {
        public long Id { get; set; }
        public long CodigoAluno { get; set; }
        public long NotificacaoId { get; set; }
        public DateTime CriadoEm { get; set; }
        public void InserirAuditoria()
        {
            CriadoEm = DateTime.Now;
        }
    }
}
