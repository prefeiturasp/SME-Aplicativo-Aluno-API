using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    class UsuarioCoreSSO
    {
        public Guid Id { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public IEnumerable<Guid> Grupos { get; set; }
    }
}
