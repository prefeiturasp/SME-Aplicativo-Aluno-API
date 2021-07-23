using Microsoft.Data.SqlClient;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos
{
    public interface IUsuarioSenhaHistoricoCoreSSORepositorio : IExternoRepositorio<UsuarioSenhaHistoricoCoreSSO, SqlConnection>
    {
        Task<bool> VerificarUltimas5Senhas(Guid usuId, string senha);

        Task AdicionarSenhaHistorico(UsuarioSenhaHistoricoCoreSSO usuarioSenhaHistoricoCoreSSO);
    }
}
