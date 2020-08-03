using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Dominio.Entidades
{
    public class NotificacaoTurma
    {
        public long Id { get; set; }
        public long CodigoTurma { get; set; }
        public long NotificacaoId { get; set; }
        public DateTime CriadoEm { get; set; }
        public void InserirAuditoria()
        {
            CriadoEm = DateTime.Now;
        }
    }
}
