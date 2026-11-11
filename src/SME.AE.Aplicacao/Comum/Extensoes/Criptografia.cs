using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Extensoes
{
    [ExcludeFromCodeCoverage]
    class Criptografia
    {
        private static readonly byte[] tdesKey = [107, 8, 82, 60, 113, 135, 190, 128, 188, 51, 238, 120, 59, 135, 57, 140, 107, 8, 82, 60, 113, 135, 190, 128];
        private static readonly byte[] tdesIV = [113, 135, 190, 128, 186, 217, 34, 47];

        protected Criptografia()
        {
        }

        public static string CriptografarSenhaSHA512(string senha)
        {
            return Convert.ToBase64String(SHA512.HashData(Encoding.Unicode.GetBytes(senha))).TrimStart('/');
        }

        public static bool EqualsSenha(string senha1, string senha2, TipoCriptografia tipo)
        {
            if (tipo == TipoCriptografia.TripleDES)
                return CriptografarSenhaTripleDES(senha1) == senha2;
            else
                return CriptografarSenhaSHA512(senha1) == senha2;
        }

        public static string CriptografarSenhaTripleDES(string senha)
        {
            byte[] plainByte = ASCIIEncoding.ASCII.GetBytes(senha);
            MemoryStream ms = new MemoryStream();
#pragma warning disable S5547
            SymmetricAlgorithm sym = TripleDES.Create();
#pragma warning restore S5547
            CryptoStream encStream = new CryptoStream(ms, sym.CreateEncryptor(tdesKey, tdesIV), CryptoStreamMode.Write);
            encStream.Write(plainByte, 0, plainByte.Length);
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            return Convert.ToBase64String(cryptoByte);
        }

        public static string DescriptografarSenhaTripleDES(string senha)
        {
            byte[] cryptoByte = Convert.FromBase64String(senha);
#pragma warning disable S5547
            var sym = TripleDES.Create();
#pragma warning restore S5547
            MemoryStream ms = new(cryptoByte, 0, cryptoByte.Length);
            CryptoStream cs = new(ms, sym.CreateDecryptor(tdesKey, tdesIV), CryptoStreamMode.Read);
            var ret = ReadBytes(cs);
            return Encoding.ASCII.GetString(ret);
        }

        public static string CriptografarSenha(string senha, TipoCriptografia tipo)
        {
            switch (tipo)
            {
                case TipoCriptografia.TripleDES:
                    return CriptografarSenhaTripleDES(senha);
                case TipoCriptografia.MD5:
                    throw new NotImplementedException();
                case TipoCriptografia.SHA512:
                    return CriptografarSenhaSHA512(senha);
                default:
                    throw new NotImplementedException();
            }
        }

        private static byte[] ReadBytes(Stream s)
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
