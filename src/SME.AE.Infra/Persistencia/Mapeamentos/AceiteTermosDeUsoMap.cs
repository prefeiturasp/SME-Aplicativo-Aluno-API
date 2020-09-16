using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class AceiteTermosDeUsoMap : BaseMap<AceiteTermosDeUso>
    {
        public AceiteTermosDeUsoMap() : base()
        {
            ToTable("aceite_termos_de_uso");
            Map(x => x.CpfUsuario).ToColumn("cpf_usuario");
            Map(x => x.Device).ToColumn("device");
            Map(x => x.Ip).ToColumn("ip");
            Map(x => x.Versao).ToColumn("versao");
            Map(x => x.DataHoraAceite).ToColumn("data_hora_aceite");
            Map(x => x.Hash).ToColumn("hash");
        }
    }
}
