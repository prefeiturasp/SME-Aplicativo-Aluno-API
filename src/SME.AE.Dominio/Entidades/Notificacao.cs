using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SME.AE.Dominio.Entidades
{
   [Table("notificacao")]
    public  class Notificacao 
    {
        public long Id { get; set; }
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public string Grupo { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime DataExpiracao { get; set; }
        public DateTime? CriadoEm { get; set; }
        public string CriadoPor { get; set; }
        public DateTime? AlteradoEm { get; set; }
        public string? AlteradoPor { get; set; }
    }
}
