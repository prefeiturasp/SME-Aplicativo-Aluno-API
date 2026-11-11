using System.IO;

namespace SME.AE.Aplicacao.Comum.Extensoes
{
    internal static class CriptografiaHelpers
    {

        private static byte[] _ReadBytes(Stream s)
        {
            int length = 10000000;
            byte[] buffer = new byte[length];
            int bytesLidos = length;
            using MemoryStream ms = new();
            while (bytesLidos == length)
            {
                bytesLidos = s.Read(buffer, 0, length);
                ms.Write(buffer, 0, bytesLidos);
            }

            return ms.ToArray();
        }
    }
}