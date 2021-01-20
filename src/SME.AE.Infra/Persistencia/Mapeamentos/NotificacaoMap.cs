using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class NotificacaoMap : BaseMap<Notificacao>
    {
        public NotificacaoMap()
        {
            ToTable("notificacao");

            Map(x => x.Id).ToColumn("id").IsKey();
            Map(x => x.Titulo).ToColumn("titulo");
            Map(x => x.Mensagem).ToColumn("mensagem");
            Map(x => x.Grupo).ToColumn("grupo");
            Map(x => x.DataEnvio).ToColumn("dataenvio");
            Map(x => x.DataExpiracao).ToColumn("dataexpiracao");
            Map(x => x.CodigoDre).ToColumn("dre_codigoeol");
            Map(x => x.CodigoUe).ToColumn("ue_codigoeol");
            Map(x => x.AnoLetivo).ToColumn("ano_letivo");
            Map(x => x.TipoComunicado).ToColumn("tipocomunicado");
            Map(x => x.CategoriaNotificacao).ToColumn("categorianotificacao");
            Map(x => x.SeriesResumidas).ToColumn("seriesresumidas");
            Map(x => x.Modalidades).ToColumn("modalidades");
        }
    }
}
