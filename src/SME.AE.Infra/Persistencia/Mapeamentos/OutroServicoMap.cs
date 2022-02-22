using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class OutroServicoMap : BaseMap<OutroServico>
    {
        public OutroServicoMap() : base()
        {
            ToTable("outroservico");
            Map(x => x.Titulo).ToColumn("titulo");
            Map(x => x.Descricao).ToColumn("descricao");
            Map(x => x.Categoria).ToColumn("categoria");
            Map(x => x.UrlSite).ToColumn("urlsite");
            Map(x => x.Icone).ToColumn("icone");
            Map(x => x.Destaque).ToColumn("destaque");
            Map(x => x.Ativo).ToColumn("ativo");
        }
    }
}
