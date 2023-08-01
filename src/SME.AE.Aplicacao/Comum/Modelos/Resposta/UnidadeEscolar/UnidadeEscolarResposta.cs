namespace SME.AE.Aplicacao.Comum.Modelos.Resposta.UnidadeEscolar
{
    public class UnidadeEscolarResposta
    {
        public string Nome { get; set; }
        public string NomeExibicao { get; set; }
        public string Codigo { get; set; }
        public string TipoLogradouro { get; set; }
        public string Logradouro { get; set; }
        public string Numero { get; set; }
        public string Bairro { get; set; }
        public int Cep { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string NomeCompletoUe => $"{Codigo} - {NomeExibicao}";
    }
}
