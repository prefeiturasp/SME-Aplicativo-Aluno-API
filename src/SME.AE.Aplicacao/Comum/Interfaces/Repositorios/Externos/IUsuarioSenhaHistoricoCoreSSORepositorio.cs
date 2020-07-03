using Microsoft.Data.SqlClient;
using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos
{
    public interface IUsuarioSenhaHistoricoCoreSSORepositorio : IExternoRepositorio<UsuarioSenhaHistoricoCoreSSO, SqlConnection>
    {
        Task<bool> VerificarUltimas5Senhas(string usuId, string senha);

        Task AdicionarSenhaHistorico(UsuarioSenhaHistoricoCoreSSO usuarioSenhaHistoricoCoreSSO);
    }
}
