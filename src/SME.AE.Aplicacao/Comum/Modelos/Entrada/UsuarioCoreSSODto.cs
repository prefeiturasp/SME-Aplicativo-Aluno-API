using SME.AE.Aplicacao.Comum.Extensoes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Modelos.Entrada
{
    public class UsuarioCoreSSODto
    {
        public UsuarioCoreSSODto()
        {
            Grupos = new List<Guid>();
        }

        public string Cpf { get; set; }
        public string Nome { get; set; }
        public string Senha { get; set; }
        public string SenhaCriptografada
        {
            get
            {
                return Criptografia.CriptografarSenha(Senha, Enumeradores.TipoCriptografia.TripleDES);
            }
        }

        public List<Guid> Grupos { get; set; }
    }
}
