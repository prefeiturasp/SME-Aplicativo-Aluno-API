using System;

namespace SME.AE.Aplicacao.Comum.Modelos
{
    public class RetornoUsuarioCoreSSO
    {
        public Guid UsuId { get; set; }
        public string Cpf { get; set; }
        public string Senha { get; set; }
        public Guid GrupoId { get; set; }
        public int Status { get; internal set; }
    }
}
