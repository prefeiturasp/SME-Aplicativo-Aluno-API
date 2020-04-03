using System;

namespace SME.AE.Aplicacao.Comum.Config
{
    public static class ConnectionStrings
    {
        public static string Conexao = Environment.GetEnvironmentVariable("AEConnection");
        public static string ConexaoEol = Environment.GetEnvironmentVariable("EolConnection");
        public static string ConexaoSgp = Environment.GetEnvironmentVariable("SgpConnection");
        public static string ConexaoCoreSSO = Environment.GetEnvironmentVariable("CoreSSOConnection");
    }
}
