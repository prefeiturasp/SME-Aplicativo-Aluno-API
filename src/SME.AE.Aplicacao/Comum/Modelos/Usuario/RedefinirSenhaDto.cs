using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Usuario
{
    public class RedefinirSenhaDto
    {
        public string Token { get; set; }
        public string Senha { get; set; }
        public string DispositivoId { get; set; }
    }
}
