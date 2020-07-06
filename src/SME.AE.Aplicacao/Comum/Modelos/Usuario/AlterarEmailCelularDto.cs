
using System.Text.RegularExpressions;

namespace SME.AE.Aplicacao.Comum.Modelos.Usuario
{
    public class AlterarEmailCelularDto
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
        public bool AlterarSenha { get; set; }

        public string CelularBanco
        {
            get
            {
                var apenasDigitos = new Regex(@"[^\d]");
                return apenasDigitos.Replace(Celular, "");
            }
        }
    }
}

