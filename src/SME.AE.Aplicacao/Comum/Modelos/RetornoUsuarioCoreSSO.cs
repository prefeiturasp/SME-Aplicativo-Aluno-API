using SME.AE.Aplicacao.Comum.Enumeradores;
using SME.AE.Aplicacao.Comum.Extensoes;
using SME.AE.Comum.Excecoes;
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

        public void AlterarSenha(string senha)
        {
            Senha = ObterSenhaCriptografada(senha);
        }

        public void ValidarSenhaAlterarSenha(string senha)
        {
            if (!senha.Equals(Senha))
                throw new NegocioException("A senha anterior não é igual a senha informada");
        }

        public string ObterSenhaCriptografada(string senha)
        {
            return Criptografia.CriptografarSenha(senha, TipoCriptografia);
        }
    }
}
