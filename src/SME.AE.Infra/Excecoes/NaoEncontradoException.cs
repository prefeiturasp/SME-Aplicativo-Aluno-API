using System;

namespace SME.AE.Infra.Excecoes
{
    public class NaoEncontradoException : Exception
    {
        public NaoEncontradoException(string name, object key)
            : base($"Entidade \"{name}\" ({key}) não foi encontrada.")
        {
        }
    }
}
