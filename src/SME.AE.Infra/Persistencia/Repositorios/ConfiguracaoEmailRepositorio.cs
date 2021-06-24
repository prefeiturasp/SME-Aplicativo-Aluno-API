using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Comum;
using SME.AE.Dominio.Entidades;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ConfiguracaoEmailRepositorio : BaseRepositorio<ConfiguracaoEmail>, IConfiguracaoEmailRepositorio
    {
        public ConfiguracaoEmailRepositorio(VariaveisGlobaisOptions variaveisGlobais) : base(variaveisGlobais.AEConnection)
        {

        }
    }
}
