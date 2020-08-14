using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comandos.Usuario.SalvarUsuario
{
    public class SalvarUsuarioCommand : IRequest
    {
        public SalvarUsuarioCommand(Dominio.Entidades.Usuario usuario)
        {
            Usuario = usuario;
        }

        public Dominio.Entidades.Usuario Usuario { get; set; }
    }
}
