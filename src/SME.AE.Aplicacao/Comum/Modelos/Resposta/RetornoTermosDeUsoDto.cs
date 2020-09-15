namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class RetornoTermosDeUsoDto
    {
        public long Id { get; set; }
        public string TermosDeUso { get; set; }
        public string PoliticaDePrivacidade { get; set; }
        public double Versao { get; set; }

        public RetornoTermosDeUsoDto(string termosDeUso, string politicaDePrivacidade, double versao, long id)
        {
            TermosDeUso = termosDeUso;
            PoliticaDePrivacidade = politicaDePrivacidade;
            Versao = versao;
            Id = id;
        }
    }
}