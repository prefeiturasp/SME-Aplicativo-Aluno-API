using System;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario.Excecoes
{
    public class UsuarioNaoEncontradoException : Exception
    {
        public UsuarioNaoEncontradoException() : base("Usuário não encontrado!")
        {
        }
    }
}
