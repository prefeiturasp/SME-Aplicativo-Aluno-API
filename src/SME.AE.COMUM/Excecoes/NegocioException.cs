using System;

namespace SME.AE.Comum.Excecoes
{
    public class NegocioException : Exception
    {
        public int StatusCode { get; set; }

        public NegocioException(string mensagem, int statusCode = 400) : base(mensagem)
        {
            StatusCode = statusCode;
        }
    }
}
