using MediatR;
using SME.AE.Aplicacao.Comum.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Consultas.ObterUsuarioCoreSSO
{
    public class ObterUsuarioCoreSSOQuery : IRequest<RetornoUsuarioCoreSSO>
    {
        public ObterUsuarioCoreSSOQuery(Guid usuarioId)
        {
            UsuarioId = usuarioId;
        }

        public ObterUsuarioCoreSSOQuery(string cpf)
        {
            Cpf = cpf;
        }

        public Guid UsuarioId { get; set; }
        public string Cpf { get; set; }
    }
}
