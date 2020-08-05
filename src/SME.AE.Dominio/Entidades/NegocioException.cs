using System;
using System.Runtime.Serialization;

namespace SME.AE.Dominio.Entidades
{
    [Serializable]
    internal class NegocioException : Exception
    {
        public NegocioException()
        {
        }

        public NegocioException(string message) : base(message)
        {
        }

        public NegocioException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NegocioException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}