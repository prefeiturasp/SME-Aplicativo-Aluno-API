using System;

namespace SME.AE.Dominio.Entidades
{
    public class Usuario
    {
        public long Id { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public DateTime UltimoLogin { get; set; }
    }
}
