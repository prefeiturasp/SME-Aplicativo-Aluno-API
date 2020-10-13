namespace SME.AE.Aplicacao.Comum.Modelos.Resposta
{
    public class RetornoToken
    {
        public string Token { get; set; }

        public RetornoToken(string token)
        {
            Token = token;
        }
    }
}
