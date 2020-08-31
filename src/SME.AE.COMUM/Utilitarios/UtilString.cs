using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Comum.Utilitarios
{
    public static class UtilString
    {
        public static string EncodeUTF8(string texto)
        {
            var utf8 = new UTF8Encoding();

            var encodedBytes = utf8.GetBytes(texto);

            return utf8.GetString(encodedBytes);
        }
    }
}
