namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class UsuarioDto
    {
        public string Cpf { get; set; }
        public string Nome { get; set; }

        public UsuarioDto(string cpf, string nome)
        {
            Cpf = cpf;
            Nome = nome;
        }
    }
}