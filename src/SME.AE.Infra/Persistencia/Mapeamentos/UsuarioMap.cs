using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class UsuarioMap : BaseMap<Usuario>
    {
        public UsuarioMap() : base()
        {
            ToTable("usuario");
            Map(x => x.Cpf).ToColumn("cpf");
            Map(x => x.Nome).ToColumn("nome");
            Map(x => x.Excluido).ToColumn("excluido");
            Map(x => x.Email).ToColumn("email");
            Map(x => x.UltimoLogin).ToColumn("ultimologin");
            Map(x => x.PrimeiroAcesso).ToColumn("primeiroacesso");
            Map(x => x.Celular).ToColumn("celular");
        }
    }
}
