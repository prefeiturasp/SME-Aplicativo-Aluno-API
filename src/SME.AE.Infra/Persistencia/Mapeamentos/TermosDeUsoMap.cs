using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class TermosDeUsoMap : BaseMap<TermosDeUso>
    {
        public TermosDeUsoMap() : base()
        {
            ToTable("termos_de_uso");
            Map(x => x.DescricaoTermosDeUso).ToColumn("descricao_termos_uso");
            Map(x => x.DescricaoPoliticaPrivacidade).ToColumn("descricao_politica_privacidade");
            Map(x => x.Versao).ToColumn("versao");
        }
    }
}
