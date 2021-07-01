using System;

namespace SME.AE.Aplicacao
{
    public class UsuarioDadosDetalhesDto
    {
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Email { get; set; }
        public DateTime? DataNascimento { get; set; }
        public string NomeMae { get; set; }
        public string Celular { get; set; }
        public DateTime? UltimaAtualizacao { get; set; }
    }
}