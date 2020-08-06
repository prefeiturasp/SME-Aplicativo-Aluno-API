using MediatR;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenAutenticacao
{
    public class ObterUsuarioPorTokenAutenticacaoCommand : IRequest<Usuario>
    {
        public ObterUsuarioPorTokenAutenticacaoCommand(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
