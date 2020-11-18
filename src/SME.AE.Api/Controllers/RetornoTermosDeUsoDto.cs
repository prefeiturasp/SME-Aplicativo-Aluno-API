namespace SME.AE.Api.Controllers
{
    public class RetornoTermosDeUsoDto
    {
        public string TermosDeUso { get; set; }
        public string PoliticaDePrivacidade { get; set; }
        public string Versao { get; set; }

        public RetornoTermosDeUsoDto(string termosDeUso, string politicaDePrivacidade, string versao)
        {
            TermosDeUso = termosDeUso;
            PoliticaDePrivacidade = politicaDePrivacidade;
            Versao = versao;
        }
    }
}