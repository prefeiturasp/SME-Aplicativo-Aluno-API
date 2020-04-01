using System;

namespace SME.AE.Aplicacao.Compartilhado
{
    public static class ConnectionStrings
    {
        public static string ConexaoEol= Environment.GetEnvironmentVariable("EolConnection");
        public static string ConexaoCoreSSO = Environment.GetEnvironmentVariable("CoreSSOConnection");
        public static string Conexao = Environment.GetEnvironmentVariable("AEConnection");
    }
}
