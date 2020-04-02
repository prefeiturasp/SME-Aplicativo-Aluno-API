using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Config
{
    public static class Configs
    {
        public static string Secret = Environment.GetEnvironmentVariable("AE_Secret");
    }
}
