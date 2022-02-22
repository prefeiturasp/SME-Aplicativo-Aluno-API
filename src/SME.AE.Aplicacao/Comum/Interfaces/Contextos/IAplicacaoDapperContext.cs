using System.Data;

namespace SME.AE.Aplicacao.Comum.Interfaces.Contextos
{
    public interface IAplicacaoDapperContext<T> where T : IDbConnection
    {
        IDbConnection Conexao { get; }
    }
}
