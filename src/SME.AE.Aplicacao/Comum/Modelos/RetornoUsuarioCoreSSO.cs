using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class RetornoUsuarioCoreSSO
    {
        public Guid UsuId { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public IEnumerable<Guid> Grupos { get; set; }
        public int Status { get; internal set; }
        public TipoCriptografia TipoCriptografia {get;set;}
    }
}
