using SME.AE.Aplicacao.Comum.Config;
using SME.AE.Aplicacao.Comum.Interfaces.Repositorios;
using SME.AE.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Infra.Persistencia.Repositorios
{
    public class ConfiguracaoEmailRepositorio : BaseRepositorio<ConfiguracaoEmail>, IConfiguracaoEmailRepositorio
    {
        public ConfiguracaoEmailRepositorio() : base(ConnectionStrings.Conexao)
        {

        }
    }
}
