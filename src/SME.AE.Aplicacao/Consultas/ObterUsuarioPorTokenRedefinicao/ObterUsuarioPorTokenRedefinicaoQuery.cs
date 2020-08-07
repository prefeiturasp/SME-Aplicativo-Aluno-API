using MediatR;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioPorTokenRedefinicao
{
    public class ObterUsuarioPorTokenRedefinicaoQuery : IRequest<Usuario>
    {
        public ObterUsuarioPorTokenRedefinicaoQuery(string token)
        {
            Token = token;
        }

        public string Token { get; set; }
    }
}
