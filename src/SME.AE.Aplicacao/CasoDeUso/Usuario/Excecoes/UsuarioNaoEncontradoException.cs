﻿using SME.AE.Comum.Excecoes;

namespace SME.AE.Aplicacao.CasoDeUso.Usuario.Excecoes
{
    public class UsuarioNaoEncontradoException : NegocioException
    {
        public UsuarioNaoEncontradoException() : base("Usuário não encontrado!")
        {
        }
    }
}
