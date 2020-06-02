using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Interfaces.Contextos
{
    public interface IAplicacaoDapperContext : IDbConnection
    {
        IDbConnection Conexao { get; }
    }
}
