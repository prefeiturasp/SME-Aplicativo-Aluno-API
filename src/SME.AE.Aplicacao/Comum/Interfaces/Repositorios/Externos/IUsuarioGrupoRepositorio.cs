using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos
{
    public interface IUsuarioGrupoRepositorio : IExternoRepositorio<UsuarioGrupoCoreSSO, SqlConnection>
    {
        Task<IEnumerable<UsuarioGrupoCoreSSO>> ObterPorUsuarioId(Guid usuarioId);
    }
}
