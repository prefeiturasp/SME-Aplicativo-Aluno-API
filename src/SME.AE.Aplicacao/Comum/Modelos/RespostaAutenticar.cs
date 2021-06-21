using System;

namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class RespostaAutenticar 
    {
        public long Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string NomeMae { get; set; }
        public string Token { get; set; }
        public bool PrimeiroAcesso { get; internal set; }
        public bool AtualizarDadosCadastrais { get; set; }
        public object Celular { get; internal set; }
        public DateTime? UltimaAtualizacao { get; set; }
    }
}
