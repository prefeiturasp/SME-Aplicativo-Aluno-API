using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SME.AE.Dominio.Entidades
{
    [Table("usuario")]
    public class Usuario : EntidadeBase
    {
        public string? Cpf { get; set; }
        public string? Celular { get; set; }
        public string Nome { get; set; }
        public string? Email { get; set; }
        public string? Celular { get; set; }
        public DateTime UltimoLogin { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool Excluido { get; set; }
    }
}
