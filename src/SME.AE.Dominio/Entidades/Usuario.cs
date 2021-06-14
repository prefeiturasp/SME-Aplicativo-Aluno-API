using SME.AE.Comum.Excecoes;
using System;

namespace SME.AE.Dominio.Entidades
{  
    public class Usuario : EntidadeBase
    {
        public string Cpf { get; set; }
        public DateTime UltimoLogin { get; set; }
        public bool PrimeiroAcesso { get; set; }
        public bool Excluido { get; set; }
        public bool RedefinirSenha { get; set; }
        public string Token { get; set; }
        public DateTime? ValidadeToken { get; set; }

        public void AtualizarLogin(bool primeiroAcesso = false)
        {
            UltimoLogin = DateTime.Now;
            Excluido = false;
            PrimeiroAcesso = primeiroAcesso;
        }

        public void IniciarRedefinicaoSenha()
        {
            RedefinirSenha = true;
            Token = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            ValidadeToken = DateTime.Now.AddHours(6);
        }

        public void ValidarTokenRedefinicao(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new NegocioException("Deve ser obrigátorio informar o Codigo de Verificação");

            if (ValidadeToken < DateTime.Now)
                throw new NegocioException("Codigo de Verificação expirado");

            if (!token.Equals(Token))
                throw new NegocioException("Codigo de Verificação inválido");
        }

        public void FinalizarRedefinicaoSenha()
        {
            RedefinirSenha = false;
            Token = string.Empty;
            ValidadeToken = null;
        }
    }
}
