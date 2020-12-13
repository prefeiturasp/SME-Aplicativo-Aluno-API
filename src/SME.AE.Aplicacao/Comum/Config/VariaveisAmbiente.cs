using System;

namespace SME.AE.Aplicacao.Comum.Config
{
    public static class VariaveisAmbiente
    {
        public static string JwtTokenSecret { get => Environment.GetEnvironmentVariable("SME_AE_JWT_TOKEN_SECRET"); }
        public static string ChaveIntegracao { get => Environment.GetEnvironmentVariable("ChaveIntegracao"); }
        public static string SentryDsn { get => Environment.GetEnvironmentVariable("Sentry__Dsn"); }
        public static string FirebaseToken { get => Environment.GetEnvironmentVariable("FirebaseToken"); }
        public static string FirebaseProjectId { get => Environment.GetEnvironmentVariable("FirebaseProjectId"); }
        public static string UrlArquivosEstaticos { get => Environment.GetEnvironmentVariable("UrlFrontEnd"); }
    }
}
