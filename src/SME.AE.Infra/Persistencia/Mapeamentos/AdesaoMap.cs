using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public class AdesaoMap : BaseMap<Adesao>
    {
        public AdesaoMap() : base()
        {
            ToTable("usuario");
            Map(x => x.Id).ToColumn("id");
            Map(x => x.CodigoDre).ToColumn("dre_codigo");
            Map(x => x.NomeDre).ToColumn("dre_nome");
            Map(x => x.CodigoUe).ToColumn("ue_codigo");
            Map(x => x.NomeUe).ToColumn("ue_nome");
            Map(x => x.CodigoTurma).ToColumn("codigo_turma");
            Map(x => x.UsuariosPrimeiroAcessoIncompleto).ToColumn("usuarios_primeiro_acesso_incompleto");
            Map(x => x.UsuariosValidos).ToColumn("usuarios_validos");
            Map(x => x.UsuariosCpfInvalidos).ToColumn("usuarios_cpf_invalidos");
            Map(x => x.UsuariosSemAppInstalado).ToColumn("usuarios_sem_app_instalado");
        }
    }
}
