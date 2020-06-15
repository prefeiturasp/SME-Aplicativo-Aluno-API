using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Excecoes
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
