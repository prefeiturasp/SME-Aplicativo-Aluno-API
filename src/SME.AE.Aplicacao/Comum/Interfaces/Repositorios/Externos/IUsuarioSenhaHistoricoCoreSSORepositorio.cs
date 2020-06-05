using SME.AE.Dominio.Entidades.Externas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SME.AE.Aplicacao.Comum.Interfaces.Repositorios.Externos
{
    public interface IUsuarioSenhaHistoricoCoreSSORepositorio
    {
        Task<bool> VerificarUltimas5Senhas(string usuId, string senha);
    }
}
