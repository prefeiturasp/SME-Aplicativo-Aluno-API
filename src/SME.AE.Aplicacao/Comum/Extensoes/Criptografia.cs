using SME.AE.Aplicacao.Comum.Enumeradores;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Extensoes
{
    class Criptografia
    {
        private static readonly byte[] tdesKey = new byte[] { 107, 8, 82, 60, 113, 135, 190, 128, 188, 51, 238, 120, 59, 135, 57, 140, 107, 8, 82, 60, 113, 135, 190, 128 };
        private static readonly byte[] tdesIV = new byte[] { 113, 135, 190, 128, 186, 217, 34, 47 };

        public static string CriptografarSenhaSHA512(string senha)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                return Convert.ToBase64String(sha512.ComputeHash(Encoding.Unicode.GetBytes(senha))).TrimStart('/');
            }
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
            SymmetricAlgorithm sym = TripleDES.Create();
            CryptoStream encStream = new CryptoStream(ms, sym.CreateEncryptor(tdesKey, tdesIV), CryptoStreamMode.Write);
            encStream.Write(plainByte, 0, plainByte.Length);
            encStream.FlushFinalBlock();
            byte[] cryptoByte = ms.ToArray();
            return Convert.ToBase64String(cryptoByte);
        }

        public static string DescriptografarSenhaTripleDES(string senha)
        {
            byte[] cryptoByte = Convert.FromBase64String(senha);
            var sym = TripleDES.Create();
            MemoryStream ms = new MemoryStream(cryptoByte, 0, cryptoByte.Length);
            CryptoStream cs = new CryptoStream(ms, sym.CreateDecryptor(tdesKey, tdesIV), CryptoStreamMode.Read);
            var ret = _ReadBytes(cs);
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

        private static byte[] _ReadBytes(Stream s)
        {
            int length = 10000000;
            byte[] buffer = new byte[length];
            int bytesLidos = length;
            using (MemoryStream ms = new MemoryStream())
            {
                while (bytesLidos == length)
                {
                    bytesLidos = s.Read(buffer, 0, length);
                    ms.Write(buffer, 0, bytesLidos);
                }

                return ms.ToArray();
            }
        }
    }
}
