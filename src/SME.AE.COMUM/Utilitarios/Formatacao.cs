using System;

namespace SME.AE.Comum.Utilitarios
{
    public static class Formatacao
    {
        public static string FormatarCpf(string CPF)
        {
            return Convert.ToUInt64(CPF).ToString(@"000\.000\.000\-00");
        }
    }
}
