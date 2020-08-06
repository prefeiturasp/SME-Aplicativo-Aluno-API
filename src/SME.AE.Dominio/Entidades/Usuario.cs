using System;

namespace SME.AE.Dominio.Entidades
{  
    public class Usuario : EntidadeBase
    {
        public string? Cpf { get; set; }
        public string? Celular { get; set; }
        public string Nome { get; set; }
        public string? Email { get; set; }
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

        public void InicarRedefinicaoSenha()
        {
            RedefinirSenha = true;
            Token = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            ValidadeToken = DateTime.Now.AddHours(6);
        }

        public bool ValidarTokenRedefinicao(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new NegocioException("Deve ser obrigátorio informar o token");

            return token.Equals(Token);
        }

        public void FinalizarRedefinicaoSenha()
        {
            RedefinirSenha = false;
            Token = string.Empty;
            ValidadeToken = null;
        }
    }
}
