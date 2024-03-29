﻿namespace SME.AE.Comum
{
    public class VariaveisGlobaisOptions
    {
        public string ApiPalavrasBloqueadas { get; set; }

        public string SME_AE_JWT_TOKEN_SECRET { get; set; }

        public string AEConnection { get; set; }

        public string CoreSSOConnection { get; set; }

        public string ChaveIntegracao { get; set; }

        public string FirebaseToken { get; set; }

        public string FirebaseProjectId { get; set; }

        public string SentryDsn { get; set; }
        public string UrlArquivosEstaticos { get; set; }

        public string ApiSgp { get; set; }
        public string ChaveIntegracaoSgpApi { get; set; }
        public string ApiEol { get; set; }
        public string ChaveIntegracaoEolApi { get; set; }
    }
}
