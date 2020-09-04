using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioDto
    {
        public string Cpf { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Grupo { get; set; }
    }
}
