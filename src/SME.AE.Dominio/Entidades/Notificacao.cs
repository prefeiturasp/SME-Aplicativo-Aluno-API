using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SME.AE.Dominio.Entidades
{
    [Table("notificacao")]
    public class Notificacao : EntidadeBase
    {
        public string Mensagem { get; set; }
        public string Titulo { get; set; }
        public string Grupo { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime? DataExpiracao { get; set; }
    }
}
