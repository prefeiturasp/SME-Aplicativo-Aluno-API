using Microsoft.Data.SqlClient;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos
{
    public interface IPessoaCoreSSORepositorio : IExternoRepositorio<PessoaCoreSSO, SqlConnection>
    {
    }
}
