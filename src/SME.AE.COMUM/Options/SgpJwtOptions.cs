namespace SME.AE.Comum
{
    public class SgpJwtOptions
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string ExpiresInMinutes { get; set; }

        public string IssuerSigningKey { get; set; }       
    }
}
