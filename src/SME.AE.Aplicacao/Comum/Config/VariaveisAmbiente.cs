using System;

namespace SME.AE.Aplicacao.Comum.Config
{
    public static class VariaveisAmbiente
    {
        public static string JwtTokenSecret = Environment.GetEnvironmentVariable("SME_AE_JWT_TOKEN_SECRET");
        public static string ChaveIntegracao = Environment.GetEnvironmentVariable("ChaveIntegracao");
        public static string SentryDsn = Environment.GetEnvironmentVariable("SentryDsn");
        public static string WorkerSentryDsn = Environment.GetEnvironmentVariable("WorkerSentryDsn");
        public static string FirebaseToken = Environment.GetEnvironmentVariable("FirebaseToken");
        public static string FirebaseProjectId = Environment.GetEnvironmentVariable("FirebaseProjectId");
        public static string UrlArquivosEstaticos = Environment.GetEnvironmentVariable("UrlFrontEnd");

    }
}
