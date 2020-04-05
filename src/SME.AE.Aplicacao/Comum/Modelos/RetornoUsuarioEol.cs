using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class RetornoUsuarioEol
    {
        public int Id { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public int TipoSigilo { get; set; }
    }
}
