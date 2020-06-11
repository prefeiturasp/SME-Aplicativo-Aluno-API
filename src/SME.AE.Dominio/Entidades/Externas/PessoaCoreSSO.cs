using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Dominio.Entidades.Externas
{
    public class PessoaCoreSSO : EntidadeExterna
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public int Situacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAlteracao { get; set; }
        public int Integridade { get; set; }
        public string NomeSocial { get; set; }
    }
}
