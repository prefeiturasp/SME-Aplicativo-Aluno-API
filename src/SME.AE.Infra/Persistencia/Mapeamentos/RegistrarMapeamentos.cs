using Dapper.FluentMap;
using Dapper.FluentMap.Dommel;
using SME.AE.Infra.Persistencia.Mapeamentos.Externos;

namespace SME.AE.Infra.Persistencia.Mapeamentos
{
    public static class RegistrarMapeamentos
    {
        public static void Registrar()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new AdesaoMap());
                config.AddMap(new UsuarioMap());
                config.AddMap(new NotificacaoMap());
                config.AddMap(new UsuarioNotificacaoMap());
                config.AddMap(new PessoaCoreSSOMap());
                config.AddMap(new UsuarioSenhaHistoricoCoreSSOMap());
                config.AddMap(new NotificacaoAlunoMap());
                config.AddMap(new NotificacaoTurmaMap());
                config.AddMap(new ConfiguracaoEmailMap());
                config.AddMap(new TermosDeUsoMap());
                config.AddMap(new AceiteTermosDeUsoMap());
                config.ForDommel();
            });
        }
    }
}
