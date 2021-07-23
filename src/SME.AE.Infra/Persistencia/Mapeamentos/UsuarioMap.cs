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
            Map(x => x.Excluido).ToColumn("excluido");
            Map(x => x.UltimoLogin).ToColumn("ultimologin");
            Map(x => x.PrimeiroAcesso).ToColumn("primeiroacesso");
            Map(x => x.Token).ToColumn("token_redefinicao");
            Map(x => x.RedefinirSenha).ToColumn("redefinicao");
            Map(x => x.ValidadeToken).ToColumn("validade_token");
        }
    }
}
